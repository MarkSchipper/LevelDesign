using UnityEngine;
using System.Collections;
using UnityEditor;

public class ProceduralBuilding : MonoBehaviour {

    public GameObject[] _baseMesh;
    public GameObject[] _middleMesh;
    public GameObject[] _upperMesh;
    public GameObject[] _roofMesh;

    private GameObject _base;
    private GameObject _middle;
    private GameObject _upper;
    private GameObject _roof;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CreateBuilding()
    {

        int _baseRandom = Random.Range(0, _baseMesh.Length);
        int _middleRandom = Random.Range(0, _middleMesh.Length);
        int _upperRandom = Random.Range(0, _upperMesh.Length);
        int _roofRandom = Random.Range(0, _roofMesh.Length);

        if (_base == null && _middle == null && _upper == null && _roof == null) {

            _base = Instantiate(_baseMesh[_baseRandom], this.transform.position, this.transform.rotation) as GameObject;
            _middle = Instantiate(_middleMesh[_middleRandom], this.transform.position, this.transform.rotation) as GameObject;
            _upper = Instantiate(_upperMesh[_upperRandom], this.transform.position, this.transform.rotation) as GameObject;
            _roof = Instantiate(_roofMesh[_roofRandom], this.transform.position, this.transform.rotation) as GameObject;

            _base.transform.parent = this.transform;
            _middle.transform.parent = this.transform;
            _upper.transform.parent = this.transform;
            _roof.transform.parent = this.transform;

        }
        else
        {

            DestroyImmediate(_base.gameObject);
            DestroyImmediate(_middle.gameObject);
            DestroyImmediate(_upper.gameObject);
            DestroyImmediate(_roof.gameObject);


            _base = Instantiate(_baseMesh[_baseRandom], this.transform.position, this.transform.rotation) as GameObject;
            _middle = Instantiate(_middleMesh[_middleRandom], this.transform.position, this.transform.rotation) as GameObject;
            _upper = Instantiate(_upperMesh[_upperRandom], this.transform.position, this.transform.rotation) as GameObject;
            _roof = Instantiate(_roofMesh[_roofRandom], this.transform.position, this.transform.rotation) as GameObject;

            _base.transform.parent = this.transform;
            _middle.transform.parent = this.transform;
            _upper.transform.parent = this.transform;
            _roof.transform.parent = this.transform;


        }
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(ProceduralBuilding))]
public class ProcBuildingEditor : Editor
{
    private ProceduralBuilding _PB;
    public override void OnInspectorGUI()
    {
        _PB = (ProceduralBuilding)target;


        base.OnInspectorGUI();
        if (GUILayout.Button("Create building!"))
        {
            _PB.CreateBuilding();

        }
    }

}
#endif
