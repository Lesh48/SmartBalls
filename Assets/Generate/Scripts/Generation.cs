using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public int[] objects_num; //количество объектов
    public int[] _max_obj;
    public int[] _min_obj;
    public int treckers_obj_num; //количество трекеров

    public float[][] default_chances_obj;

    public GameObject folder_to_find;
    public GameObject folder; // --empty in prefab

    public GameObject[][] objects;

    public SetGen[][] obj_par; // параметры объектов
    public SetTreck[] treck_par;

    public string name1_folder = "Objects";
    public string name2_folder = "Trecker";


    //set as min[high, medium, small] (can_gen), max[as previous] 
    public void Generate(int[] min_obj, int[] max_obj) // определить как минимум[высокий, средний, маленький] (может сгенерировать), максимум[как предыдущий]
    {
        _max_obj = max_obj;
        _min_obj = min_obj;

    }
    public void Generate()
    {

    }

    void _Generate_()
    {
        for(int i = 0; i < treckers_obj_num; i++)
        {
            Generate_obj();
        }
    }

    void Generate_obj()
    {
        GameObject cr_folder = Instantiate(folder);
        cr_folder.name = name1_folder;

        for(int i = 0; i < treckers_obj_num; i++)
        {
            GameObject cr_obj;
            GameObject in_folder;
            string places;
            int index = Random.Range(1, treck_par[i].num_places);

            in_folder = Instantiate(folder, cr_folder.transform);
            in_folder.name = name2_folder + (i + 1);

            Transform place = treck_par[i].gameObject.transform.Find("Pos" + i + 1);
            SetTreck place_par = place.GetComponent<SetTreck>();
            
            if (place_par.use_defaults)
            {
                place_par.objects_ar.high = default_chances_obj[0];
                place_par.objects_ar.medium = default_chances_obj[1];
                place_par.objects_ar.small = default_chances_obj[2];
            }

            cr_obj = Select_obj(place_par, null);
            places = cr_obj.name + 0;

            cr_obj = Instantiate(cr_obj, place.position, Quaternion.identity);
            cr_obj.transform.Rotate(place.transform.eulerAngles.x, place.transform.eulerAngles.y, place.transform.eulerAngles.z);
            cr_obj.transform.SetParent(in_folder.transform);

            if(place_par.num_places > 0)
            {
                index = Random.Range(1, place_par.num_places);

                place = place.transform.Find("Pos" + index);
                place_par = place.GetComponent<SetTreck>();

                if (place_par.use_defaults)
                {
                    place_par.objects_ar.high = default_chances_obj[0];
                    place_par.objects_ar.medium = default_chances_obj[1];
                    place_par.objects_ar.small = default_chances_obj[2];
                }

                cr_obj = Select_obj(place_par, places);

                cr_obj = Instantiate(cr_obj, place.position, Quaternion.identity);
                cr_obj.transform.Rotate(place.transform.eulerAngles.x, place.transform.eulerAngles.y, place.transform.eulerAngles.z);
                cr_obj.transform.SetParent(in_folder.transform);
            }



        }


    }

    GameObject Select_obj(SetTreck place_par, string places)
    {
        float s = 0;
        bool[] uses = new bool[3];
        float[][] object_chances = new float[3][];

        object_chances[0] = place_par.objects_ar.high;
        object_chances[2] = place_par.objects_ar.medium;
        object_chances[3] = place_par.objects_ar.small;

        uses[0] = place_par.objects_ar.use_high;
        uses[2] = place_par.objects_ar.use_medium;
        uses[3] = place_par.objects_ar.use_small;


        for (int i = 0; i < 3; i++)
        {
            if (uses[i])
            {
                for (int t = 0; t < objects[i].Length; i++)
                {
                    if (objects[i][t].GetComponent<SetGen>().num_gen < objects[i][t].GetComponent<SetGen>().max_can_gen)
                    {
                        s += object_chances[i][t];
                    }
                }
            }
        }

        float n = 0;

        for (int i = 0; i < 3; i++)
        {
            if (uses[i])
            {
                for (int t = 0; t < objects[i].Length; i++)
                {
                    if(objects[i][t].GetComponent<SetGen>().num_gen < objects[i][t].GetComponent<SetGen>().max_can_gen)
                    {
                        n += object_chances[i][t] / s * 100;
                        object_chances[i][t] = n;
                    }
                }
            }
        }
        object_chances[3][object_chances[3].Length - 1] = 100;

        float index = Random.Range(0.000001f, 100);

        for (int i = 0; i < 3; i++)
        {
            if (uses[i])
            {
                for (int t = 0; t < objects[i].Length; i++)
                {
                    if (index <= object_chances[i][t])
                    {
                        objects[i][t].GetComponent<SetGen>().num_gen += 1;
                        return objects[i][t];
                    }
                }
            }
        }


        if(places != null)
        {
            Debug.Log("Can't select obj, id_trecker: " + places[0] + ", " + place_par.name);
        }
        else
        {
            Debug.Log("Can't select obj, id_trecker: " + place_par.name);
        }
        
        return null;
    }


    void Get_treck_obj()
    {
        treck_par = new SetTreck[treckers_obj_num];

        GameObject folder_f = folder_to_find.transform.Find("Treckers").transform.Find("Objects").gameObject;

        for(int i = 0; i < treckers_obj_num; i++)
        {
            treck_par[i] = folder_f.transform.Find(i + 1 + "").gameObject.GetComponent<SetTreck>();
        }
        
    }


    void Start()
    {
        obj_par = new SetGen[3][];

        default_chances_obj = new float[3][];

        for (int i = 0; i < 3; i++)
        {
            obj_par[i] = new SetGen[objects[i].Length];

            default_chances_obj[i] = new float[objects[i].Length];

            for(int t = 0; t < objects[i].Length; i++)
            {
                obj_par[i][t] = objects[i][t].GetComponent<SetGen>();

                default_chances_obj[i][t] = obj_par[i][t].chances_gen;
            }

        }

    }
    /*
    Пример генерации:

        трекер
            \
          выбор места
              \
          тип-объекта
                \
                шанс[] объектов \\ шанс по умолчанию
                                 \\
                                 выбор объекета
                                    \
                                   может спавнится с
                                         /    \
                                        нет    да
                                                \
                                     место подходит объекту, шанс
                                                  \
                                                 выбор места
                                                    \
                                                 шанс[] объектов(доп об) \\ шанс по умолчанию
                                                                  \\
                                                               выбор объекта



       в иерархии:
            Treckers
                  \
                   Objects
                          \
                           1 \\ 2...
                          / \
                 SetTreck<=  pos1 \\ pos2... = SetTreck.num_poses<= (min = 1)
                    /           / \
           num_poses   SetTreck<=  pos1 \\ pos2... = SetTreck.num_poses<=
                          /            \
                 num_poses              SetTreck
    */
}
