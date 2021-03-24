using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackArmor : MonoBehaviour {

    int armorid = -1;
    int level = 0;

    //int durability = 0;
    bool ispartable = false;

    GameObject originalarmor;
    CrackArmor anotherarmor;

	public void Awake()
    {
        // ... 서버에서 받아온 armorid로 xml추출 armorid 및 level 및 방어력 설정
        // ... player에서 모든 장비 2차원배열로 가지고 있기 + crack을 구별해야할까?

        string armorname = gameObject.name;
        string[] split_text = armorname.Split('_');

        originalarmor = transform.parent.transform.parent.Find("original").Find(string.Format("{0}_{1}_{2}", split_text[0], split_text[1], split_text[2])).gameObject;

        if (split_text.Length == 4)
        {
            ispartable = true;

            if(split_text[3] == "1")
            {
                string anotherarmorname = string.Format("{0}_{1}_{2}_{3}", split_text[0], split_text[1], split_text[2], 2);
                anotherarmor = transform.parent.Find(anotherarmorname).GetComponent<CrackArmor>();
            }
            else
            {
                string anotherarmorname = string.Format("{0}_{1}_{2}_{3}", split_text[0], split_text[1], split_text[2], 1);
                anotherarmor = transform.parent.Find(anotherarmorname).GetComponent<CrackArmor>();
            }
        }

        if (originalarmor)
        {
            Debug.Log(originalarmor.name);
        }

        if (anotherarmor)
        {
            Debug.Log(this.gameObject.name + "/" + anotherarmor.name);
        }
    }
}
