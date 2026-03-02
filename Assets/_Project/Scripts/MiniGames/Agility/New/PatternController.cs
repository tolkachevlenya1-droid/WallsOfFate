using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    internal class PatternController : MonoBehaviour
    {
        public BezierSolution.BezierSpline SplinePrefab;
        public BezierSolution.BezierWalkerWithSpeed WalkerPrefab;

        public List<BezierSolution.BezierWalkerWithSpeed> SpawnedWalkers;
        public BezierSolution.BezierSpline Spline;

        public int WalkersNumber;

        public Transform SplineSpawnPoint;
        public Transform SplineParent;

        private void Awake()
        {
            SpawnSpline(SplineSpawnPoint, SplineParent);
            for (int i = 0; i < WalkersNumber; i++)
            {
                SpawnWalker(i);
            }
        }

        public void SpawnSpline(Transform spawnPoint, Transform parent)
        {
            GameObject instance = GameObject.Instantiate(SplinePrefab.gameObject, spawnPoint.position, Quaternion.identity, parent);
            Spline = instance.GetComponent<BezierSolution.BezierSpline>();
        }

        public void SpawnWalker(float delay)
        {
            StartCoroutine(SpawnWalkerCoroutine(delay));
        }

        private IEnumerator SpawnWalkerCoroutine(float delayBeforeSpawn)
        {
            if (delayBeforeSpawn > 0)
            {
                yield return new WaitForSeconds(delayBeforeSpawn);
            }

            if (WalkerPrefab != null)
            {
                GameObject instance = GameObject.Instantiate(WalkerPrefab.gameObject);
                instance.GetComponent<BezierSolution.BezierWalkerWithSpeed>().spline = this.Spline;
                SpawnedWalkers.Add(instance.GetComponent<BezierSolution.BezierWalkerWithSpeed>());

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
