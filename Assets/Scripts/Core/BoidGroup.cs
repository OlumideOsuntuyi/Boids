using System;
using System.Collections.Generic;

using Sim.Util;

using UnityEngine;

namespace Sim
{
    public class BoidGroup : MonoBehaviour
    {
        public int count;
        public BoidRules rules;
        public BoxCollider box;
        public Vector3 min, max;

        [SerializeField] private List<FishModel> models;

        public List<Boid> boids;
        private void Awake()
        {
            boids = new();
        }

        private void Update()
        {
            if (!box)
                return;

            var bb = box.bounds;
            min = bb.min;
            max = bb.max;

            while(boids.Count != count)
            {
                if(boids.Count < count)
                {
                    Vector3 randomPositionInBox = new Vector3()
                    {
                        x = Mathf.Lerp(bb.min.x, bb.max.x, Helper.RandomValue01()),
                        y = Mathf.Lerp(bb.min.y, bb.max.y, Helper.RandomValue01()),
                        z = Mathf.Lerp(bb.min.z, bb.max.z, Helper.RandomValue01())
                    };
                    boids.Add(new Boid(randomPositionInBox, models[Helper.RandomRange(0, models.Count)]));
                }
                else
                {
                    boids[0].Destroy();
                    boids.RemoveAt(0);
                }
            }

            foreach(var boid in boids)
            {
                boid.Update(boids, rules, min, max);
            }
        }

        private void OnDrawGizmos()
        {
            //if (boids == null)
            //    return;

            //foreach(var b in boids)
            //{
            //    b.OnDraw(rules);
            //}
        }
    }


    [System.Serializable]
    public class FishModel
    {
        public string name;
        public SpeciesData data;
        public GameObject prefab;
    }
}