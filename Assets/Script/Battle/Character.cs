using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character:SceneObject  {

    public enum State
    {        
        StateIdle=0,
        StateMove=1,
        StateCastSkill=2,
        StateDead=3,
        StateMoveToAttack=4,
        StateBorn=5,
    }   
  
    public Collider m_collider;

    public int m_hp=0;
    public int m_hpMax=0;

    public int m_moveSpeed;
    public int m_moveSpeedMax;

    public int m_attackSpeed;
    public int m_attackSpeedMax;

    public int m_attack=0;
    public int m_attaciMax=0;

    public int level;

    public List<Skill> mSkillIDList = new List<Skill>();

    public GameObject m_target = null;

    public virtual void Attack()
    {

    }

    public virtual void Move()
    {

    }

    public virtual void Dead()
    {

    }

}
