using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor {

	private WaveManager wv;

	int numStartCubes = 0;
	int numEnemyPrefabs = 0;
	List<int> expandedWaves = new List<int>();
	List<int> numGroups = new List<int>();

	void Awake(){
		wv = (WaveManager)target;
	}

	public override void OnInspectorGUI ()
	{

		EditorGUILayout.PropertyField (numStartCubes, new GUIContent("Start Cubes"));

		if (wv.waveData.startCubes == null) {
			wv.waveData.startCubes = new List<GameCube> ();
		}

		for (int i = 0; i < numStartCubes; i++) {
			if (i >= wv.waveData.startCubes.Count) {
				wv.waveData.startCubes.Add (null);
			}

			wv.waveData.startCubes [i] = (GameCube)EditorGUILayout.ObjectField ("Start Cube " + i, wv.waveData.startCubes [i], typeof(GameCube));
		}

		if (wv.waveData.enemyPrefabs == null) {
			wv.waveData.enemyPrefabs = new List<Enemy> ();
		}

		EditorGUILayout.PropertyField (numEnemyPrefabs, new GUIContent("Enemy Prefabs"));

		for (int i = 0; i < numEnemyPrefabs; i++) {

			if (i >= wv.waveData.enemyPrefabs.Count) {
				wv.waveData.enemyPrefabs.Add (null);
			}

			wv.waveData.enemyPrefabs [i] = (Enemy)EditorGUILayout.ObjectField ("Enemy Prefab " + i, wv.waveData.enemyPrefabs [i], typeof(Enemy));
		}



		EditorGUILayout.PropertyField (wv.waveData.enemyParent, new GUILayout("Enemy Parent"));
		wv.waveData.numberOfWaves = EditorGUILayout.IntField ("Number of Waves", wv.waveData.numberOfWaves);

		if (wv.waveData.data == null) {
			wv.waveData.data = new List<List<IncomingEnemyGroup>>();
		}

		for (int i = 0; i < wv.waveData.numberOfWaves; i++) {

			if (i >= numGroups.Count) {
				numGroups.Add (0);
			}

			if (i >= wv.waveData.data.Count) {
				wv.waveData.data.Add (new List<IncomingEnemyGroup> ());
			}
			
			bool selected = EditorGUILayout.Foldout (expandedWaves.Contains(i), "Wave " + i);

			if (selected) {

				//Custom editing for a single wave
				numGroups[i] = EditorGUILayout.IntField ("Number of Groups", numGroups[i]);

				for (int j = 0; j < numGroups[i]; j++) {

					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

					if (j >= wv.waveData.data [i].Count) {
						wv.waveData.data [i].Add( new IncomingEnemyGroup () );
					}

					/*
					wv.data[i][j].enemy = (EnemyType)EditorGUILayout.EnumPopup ("Enemy Type", wv.waveData[i][j].enemy);
					wv.data[i][j].number = EditorGUILayout.IntField ("Number", wv.data[i][j].number);
					wv.data[i][j].initialDelay = EditorGUILayout.FloatField ("Initial Delay", wv.waveData[i][j].initialDelay);
					wv.data[i][j].timeDelay = EditorGUILayout.FloatField ("Time Delay", wv.data[i][j].timeDelay);
					wv.data[i][j].startCubeIndex = EditorGUILayout.IntField ("Start Cube Index", wv.waveData[i][j].startCubeIndex);
				}
										
				if (expandedWaves.Contains (i) == false) {
					expandedWaves.Add (i);
				}

				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			} else {
				if (expandedWaves.Contains (i) == true) {
					expandedWaves.Remove (i);
				}
			}
		}

		for (int i = wv.waveData.numberOfWaves; i < numGroups.Count; i++) {
			//reset to default for extra values
			numGroups [i] = 0;
		}

		serializedObject.ApplyModifiedProperties ();
	}
}
*/