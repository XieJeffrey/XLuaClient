using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill:SceneObject {
    public int skillID;

    public GameObject m_gameObject;
    public GameObject m_target;

    public GameObject m_cd;

    public int m_damage;

    public Skill(Character owner, int skillID)
    {

    }
   
}
