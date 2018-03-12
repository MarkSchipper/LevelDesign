using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{

    public class BaseQuest : ScriptableObject
    {
        public static QuestItemAmount _qItemAmount;
        public static int _questItemIndex;
        public static int _questItemCollectAmount;
        public static string _questTitle;
        public static string _questText;
        public static string _questComplete;
        public static QuestReward _questReward;
        public static int _goldAmount;
        public static int _expAmount;
        public static QuestChain _chain;
        public static QuestChainType _chainType;
        public static int _questChainSelectIndex;
        public static bool _questEnabled;
        public static bool _questGameStart;
        public static List<GameObject> _createdQuestItems = new List<GameObject>();
        public static GameObject QuestObject;
        public static bool _questAutoComplete;
        public static QuestType _questType;

        public static int _killAmount;
        public static KillQuestSelection _killSelect;
        public static GameObject[] _killAllEnemies;
        public static List<string> _killAllEnemiesName = new List<string>();
        public static int _killSelectIndex;

        public static GameObject[] _allZones;
        public static string[] _zoneNames;
        public static int _zoneSelectedIndex;



    }
}