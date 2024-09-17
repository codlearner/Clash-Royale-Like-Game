using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 马俊航 
//功能说明: 游戏控制器
//***************************************** 
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public float energyValue;
    public List<UnitInfo> unitInfos;
    public List<GameObject> unitGos; // all units' prefab
    public Building[] orangeBuildings;
    public Building[] purpleBuildings;
    public AudioClip[] gameBGMusic;
    void Awake()
    {
        Instance = this;
        energyValue = 4;
        unitInfos = new List<UnitInfo>()
        {
            new UnitInfo(){ id=1,unitName="精灵弓箭手",cost=3,hp=10,attackArea=4,speed=1,attackValue=2},
            new UnitInfo(){ id=2,unitName="治愈天使",cost=4,hp=20,attackArea=2,speed=1,attackValue=4},
            new UnitInfo(){ id=3,unitName="三头狼",cost=6,hp=30,attackArea=2,speed=1,attackValue=6},
            new UnitInfo(){ id=4,unitName="堕天使",cost=6,hp=30,attackArea=2.5f,speed=2,attackValue=6},
            new UnitInfo(){ id=5,unitName="熔岩巨兽",cost=8,hp=40,attackArea=2,speed=1,attackValue=8},
            new UnitInfo(){ id=6,unitName="弓箭手兄弟",cost=5,hp=10,attackArea=4,speed=1,attackValue=2},
            new UnitInfo(){ id=7,unitName="装甲熊",cost=7,hp=35,attackArea=2,speed=4,attackValue=7},
            new UnitInfo(){ id=8,unitName="死神",cost=6,hp=30,attackArea=3,speed=2,attackValue=6},
            new UnitInfo(){ id=9,unitName="毒瘟疫",cost=4,attackArea=1.5f,speed=1,attackValue=1,canCreateAnywhere=true},
            new UnitInfo(){ id=10,unitName="大火球",cost=4,attackArea=2f,speed=8,attackValue=15,canCreateAnywhere=true},
            new UnitInfo(){ id=11,unitName="骷髅怪",cost=0,hp=2,attackArea=1.5f,speed=1,attackValue=1},
            new UnitInfo(){ id=12,unitName="治疗光环",cost=0,attackArea=2f,speed=8,attackValue=-2},
            new UnitInfo(){ id=13,unitName="防御塔",cost=0,hp=150,attackArea=5,speed=0,attackValue=5}
        };
        GameManager.Instance.PlayMusic(gameBGMusic[Random.Range(0,3)]);
    }

    void Update()
    {
        if (energyValue < 10)
        {
            energyValue += Time.deltaTime;
            Mathf.Clamp(energyValue,0, 10);
            UIManager.Instance.SetEnergySliderValue();
        }
    } 
    public bool CanUse(int id)
    {
        return unitInfos[id - 1].cost <= energyValue; // id starts from 1
    }
    public void DecreaseEnergyValue(int value)
    {
        energyValue -= value;
    }
    public void CreateUnit(int id,Vector3 pos,bool isPurple=false) // Orange team in default
    {
        GameObject go = Instantiate(unitGos[id - 1]);
        go.transform.position = pos;
        switch (id)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 7:
            case 8:
            case 9:
            case 11:
            case 12:
                Unit unit = go.transform.GetComponent<Unit>();
                unit.isPurple = isPurple;
                unit.unitInfo = unitInfos[id - 1];
                break;
            // because 6 has 2 archers under gameobject
            case 6:
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Unit u = go.transform.GetChild(i).GetComponent<Unit>();
                    u.isPurple = isPurple;
                    u.unitInfo = unitInfos[id - 1];
                }
                break;
            case 10:
                MagicFire fireball = go.GetComponent<MagicFire>();
                fireball.targetPos = pos;
                fireball.isPurple = isPurple;
                fireball.unitInfo = unitInfos[id-1];
                break;
            default:
                break;
        }
    }
    public void UnitGetTargetPos(Unit unit,bool isPurple) // get this unit's default target's pos
    {
        Building[] buildings = isPurple ? orangeBuildings : purpleBuildings;
        if(buildings[0] == null) return;// 0 is king building
        // index 1 is left prince, 2 is right prince
        int index = unit.transform.position.x <= buildings[0].transform.position.x ? 1 : 2;
        if(buildings[index].isDead) // if prince tower is dead
        {
            unit.defaultTarget = buildings[0]; // then king is its' default target
        }
        else
        {
            unit.defaultTarget = buildings[index]; // else this side prince is its' default target
        }
    }
    public void EnableKing(bool isPurple) // activate king tower
    {
        if (isPurple)
        {
            purpleBuildings[0].SetColliders(true);
        }
        else
        {
            orangeBuildings[0].SetColliders(true);
        }
    }
}
