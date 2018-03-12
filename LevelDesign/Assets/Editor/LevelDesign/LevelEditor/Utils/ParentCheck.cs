using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor
{
    namespace Utils
    {
        public class ParentCheck : MonoBehaviour
        {
            public static void CheckParentObjects(int _worldTabIndex, GameObject _objectToAdd, int _worldTypeTabIndex)
            {
                #region SAFEGUARDS
                //////////////////////////////////////////////////////////////////////
                //                              Safeguards                          //
                //////////////////////////////////////////////////////////////////////

                if (GameObject.Find("WORLD") == null)
                {
                    GameObject _world = new GameObject();
                    _world.name = "WORLD";
                }

                if (GameObject.Find("PROPS") == null)
                {
                    GameObject _props = new GameObject();
                    _props.name = "PROPS";
                    _props.transform.SetParent(GameObject.Find("WORLD").transform);
                }

                if (GameObject.Find("POTIONS") == null)
                {
                    GameObject _potions = new GameObject();
                    _potions.name = "POTIONS";
                    _potions.transform.SetParent(GameObject.Find("WORLD").transform);
                }
                if (GameObject.Find("STATICPROPS") == null)
                {
                    GameObject _staticProps = new GameObject();
                    _staticProps.name = "STATICPROPS";
                    _staticProps.transform.SetParent(GameObject.Find("WORLD").transform);
                }

                switch (_worldTabIndex)
                {
                    case 0:
                        if (GameObject.Find("Settlement") == null)
                        {
                            GameObject _settlementParent = new GameObject();
                            _settlementParent.name = "Settlement";
                            _settlementParent.transform.SetParent(GameObject.Find("WORLD").transform);
                        }

                        if (GameObject.Find("Settlement_Buildings") == null)
                        {
                            GameObject _buildingParent = new GameObject();
                            _buildingParent.name = "Settlement_Buildings";
                            _buildingParent.transform.SetParent(GameObject.Find("Settlement").transform);
                        }

                        if (GameObject.Find("Settlement_Perimeter") == null)
                        {
                            GameObject _perimeterParent = new GameObject();
                            _perimeterParent.name = "Settlement_Perimeter";
                            _perimeterParent.transform.SetParent(GameObject.Find("Settlement").transform);
                        }

                        if (GameObject.Find("Settlement_Props") == null)
                        {
                            GameObject _props = new GameObject();
                            _props.name = "Settlement_Props";
                            _props.transform.SetParent(GameObject.Find("Settlement").transform);
                        }

                        if (GameObject.Find("Settlement_Tiles") == null)
                        {
                            GameObject _settlementTiles = new GameObject();
                            _settlementTiles.name = "Settlement_Tiles";
                            _settlementTiles.transform.SetParent(GameObject.Find("Settlement").transform);
                        }

                        switch (_worldTypeTabIndex)
                        {
                            case 0:
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Buildings").transform);
                                break;
                            case 1:
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Tiles").transform);
                                break;
                            case 2:
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Perimeter").transform);
                                break;
                            case 3:
                                _objectToAdd.transform.SetParent(GameObject.Find("Settlement_Props").transform);
                                break;
                            default:
                                break;
                        }
                        // Theme.Settlement.DeleteLoadedObject();

                        break;
                    case 1:

                        if (GameObject.Find("Viking") == null)
                        {
                            GameObject _settlementParent = new GameObject();
                            _settlementParent.name = "Viking";
                            _settlementParent.transform.SetParent(GameObject.Find("WORLD").transform);
                        }

                        if (GameObject.Find("Viking_Buildings") == null)
                        {
                            GameObject _buildingParent = new GameObject();
                            _buildingParent.name = "Viking_Buildings";
                            _buildingParent.transform.SetParent(GameObject.Find("Viking").transform);
                        }

                        if (GameObject.Find("Viking_Perimeter") == null)
                        {
                            GameObject _perimeterParent = new GameObject();
                            _perimeterParent.name = "Viking_Perimeter";
                            _perimeterParent.transform.SetParent(GameObject.Find("Viking").transform);
                        }

                        if (GameObject.Find("Viking_Props") == null)
                        {
                            GameObject _props = new GameObject();
                            _props.name = "Viking_Props";
                            _props.transform.SetParent(GameObject.Find("Viking").transform);
                        }

                        if (GameObject.Find("Viking_Tiles") == null)
                        {
                            GameObject _settlementTiles = new GameObject();
                            _settlementTiles.name = "Viking_Tiles";
                            _settlementTiles.transform.SetParent(GameObject.Find("Viking").transform);
                        }

                        switch (_worldTypeTabIndex)
                        {
                            case 0:
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Buildings").transform);
                                break;
                            case 1:
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Tiles").transform);
                                break;
                            case 2:
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Perimeter").transform);
                                break;
                            case 3:
                                _objectToAdd.transform.SetParent(GameObject.Find("Viking_Props").transform);
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        if (GameObject.Find("Graveyard") == null)
                        {
                            GameObject _graveyard = new GameObject();
                            _graveyard.name = "Graveyard";
                            _graveyard.transform.SetParent(GameObject.Find("WORLD").transform);
                            _objectToAdd.transform.SetParent(GameObject.Find("Graveyard").transform);
                        }
                        break;
                    case 3:

                        if (GameObject.Find("Dungeon") == null)
                        {
                            GameObject _dungeonParent = new GameObject();
                            _dungeonParent.name = "Dungeon";
                            _dungeonParent.transform.SetParent(GameObject.Find("WORLD").transform);
                        }

                        if (GameObject.Find("Dungeon_Buildings") == null)
                        {
                            GameObject _buildingParent = new GameObject();
                            _buildingParent.name = "Dungeon_Buildings";
                            _buildingParent.transform.SetParent(GameObject.Find("Dungeon").transform);
                        }

                        if (GameObject.Find("Dungeon_Perimeter") == null)
                        {
                            GameObject _perimeterParent = new GameObject();
                            _perimeterParent.name = "Dungeon_Perimeter";
                            _perimeterParent.transform.SetParent(GameObject.Find("Dungeon").transform);
                        }

                        if (GameObject.Find("Dungeon_Props") == null)
                        {
                            GameObject _props = new GameObject();
                            _props.name = "Dungeon_Props";
                            _props.transform.SetParent(GameObject.Find("Dungeon").transform);
                        }

                        if (GameObject.Find("Dungeon_Tiles") == null)
                        {
                            GameObject _tiles = new GameObject();
                            _tiles.name = "Dungeon_Tiles";
                            _tiles.transform.SetParent(GameObject.Find("Dungeon").transform);
                        }

                        switch (_worldTypeTabIndex)
                        {
                            case 0:
                                _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Buildings").transform);
                                break;
                            case 1:
                                _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Tiles").transform);
                                break;
                            case 2:
                                _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Perimeter").transform);
                                break;
                            case 3:
                                _objectToAdd.transform.SetParent(GameObject.Find("Dungeon_Props").transform);
                                break;
                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
                #endregion
            }
        }
    }
}
