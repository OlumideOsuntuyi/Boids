using System.Collections;
using System.Collections.Generic;
using System.Data;

using Sim.Util;

using UnityEngine;

namespace Sim
{
    [System.Serializable]
    public class Boid
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 resultingNormal;

        private int _id;
        public SpeciesData data;
        private GameObject model;

        public Boid(Vector3 position, FishModel model)
        {
            this.position = position;
            normal = Helper.RandomDirection();
            resultingNormal = normal;

            this.model = Object.Instantiate(model.prefab);
            this.model.hideFlags = HideFlags.HideInHierarchy;
            this.model.transform.localScale = Vector3.one * 0.2f;

            data = SpeciesData.Mutate(model.data);
            _id = data.id;
        }

        public void OnDraw(BoidRules rules)
        {
            //Gizmos.color = rules.boidColor;
            //Gizmos.DrawSphere(position, rules.boidRadius);

            //Gizmos.color = rules.normalColor;
            //Gizmos.DrawLine(position, position + (normal * rules.normalLength));
        }

        public void Destroy()
        {
            Object.Destroy(model);
            model = null;
        }

        public void Update(List<Boid> boids, BoidRules rules, Vector3 bbMin, Vector3 bbMax)
        {
            // separation
            // cohesion
            // alignment
            float invCDist = 1f / rules.cohesionDistance;
            float invSDist = 1f / rules.separationDistance;
            float invADist = 1f / rules.alignmentDistance;
            float totalRuleWeights = Mathf.Max(rules.cohesionWeight + rules.separationWeight + rules.alignmentWeight);

            Vector3 totalNormal = Vector3.zero;
            Vector3 totalPosition = Vector3.zero;
            Vector3 totalHalfDistanceBetween = Vector3.zero;

            float totalCohesionWeight = 0;
            float totalSeparationWeight = 0;
            float totalAlignmentWeight = 0;

            foreach(var boid in boids)
            {
                if (boid == this)
                    continue;

                Vector3 direction = (boid.position - position).normalized;
                float dot = Vector3.Dot(direction, normal);
                if (dot < rules.fov)
                    continue;

                float distance = Vector3.Distance(position, boid.position);
                float cohesionWeight = rules.cohesionDistance > distance || (data.power > boid.data.power && _id != boid._id) ? 0 : 1;
                float separationWeight = rules.separationDistance > distance ? 0 : 1;
                float alignentWeight = rules.alignmentDistance > distance || _id != boid._id ? 0 : 1;

                cohesionWeight = Mathf.Pow(cohesionWeight, rules.cohesionStrength);
                separationWeight = Mathf.Pow(separationWeight, rules.separationStrength);
                alignentWeight = Mathf.Pow(alignentWeight, rules.alignmentStrength);

                totalNormal += boid.normal * alignentWeight;
                totalPosition += boid.position * separationWeight;
                totalHalfDistanceBetween += (boid.position - position) * 0.5f * cohesionWeight;

                totalCohesionWeight += cohesionWeight;
                totalSeparationWeight += separationWeight;
                totalAlignmentWeight += alignentWeight;
            }

            float totalBoidInfluence = totalSeparationWeight + totalAlignmentWeight + totalCohesionWeight;

            Vector3 centerPosition = totalPosition / Mathf.Max(totalSeparationWeight, 1);
            Vector3 centerNormal = totalNormal / Mathf.Max(totalAlignmentWeight, 1);
            Vector3 centerHalfDistance = totalHalfDistanceBetween / Mathf.Max(totalCohesionWeight, 1);

            Vector3 separationDirection = (position - centerPosition).normalized * rules.separationWeight; // moving away from center position
            Vector3 cohesionDirection = centerHalfDistance.normalized * rules.cohesionWeight; // moving towards center position between all boids
            Vector3 alignmentDirection = centerNormal * rules.alignmentWeight; // moving towards same direction

            Vector3 averageDirection = (separationDirection + cohesionDirection + alignmentDirection) / totalRuleWeights;
            resultingNormal = totalBoidInfluence >= rules.requiredInfluence ? Vector3.Lerp(resultingNormal, averageDirection, Time.deltaTime * rules.turnSpeed) : resultingNormal;

            UpdateMotion(rules, bbMin, bbMax);
        }

        public void UpdateMotion(BoidRules rules, Vector3 boxMin, Vector3 boxMax)
        {
            normal = resultingNormal.normalized;
            Vector3 intendedPosition = position + (Time.deltaTime * data.speed * rules.boidSpeed * normal);
            position = BoundCheck(intendedPosition, boxMin, boxMax);

            // ReverseNormals(ref normal);
            // ReverseNormals(ref resultingNormal);

            void ReverseNormals(ref Vector3 vector)
            {
                vector = new Vector3()
                {
                    x = position.x != intendedPosition.x ? -vector.x : vector.x,
                    y = position.y != intendedPosition.y ? -vector.y : vector.y,
                    z = position.z != intendedPosition.z ? -vector.z : vector.z
                };
            }

            model.transform.position = position;
            model.transform.forward = normal;
        }

        public Vector3 BoundCheck(Vector3 position, Vector3 min, Vector3 max)
        {
            // if reached edge of box, send to direct opposite end
            return new Vector3()
            {
                x = position.x >= max.x ? min.x : position.x <= min.x ? max.x : position.x,
                y = position.y >= max.y ? min.y : position.y <= min.y ? max.y : position.y,
                z = position.z >= max.z ? min.z : position.z <= min.z ? max.z : position.z
            };                    
        }

    }


    [System.Serializable]
    public class BoidRules
    {
        [Header("Rules")]
        public float cohesionDistance;
        public float cohesionStrength;

        public float separationDistance;
        public float separationStrength;

        public float alignmentDistance;
        public float alignmentStrength;

        [Range(0, 1)] public float cohesionWeight;
        [Range(0, 1)] public float separationWeight;
        [Range(0, 1)] public float alignmentWeight;

        [Range(0, 1)] public float requiredInfluence;


        [Header("Motion")]
        public float boidSpeed;
        public float turnSpeed;
        public float fov;

        [Header("Display")]
        public Color boidColor;
        public Color normalColor;
        public float boidRadius;
        public float normalLength;
    }

    [System.Serializable]
    public struct SpeciesData
    {
        public int id;
        public float hp;
        public float saturation;
        public float speed;
        public float power;
        public float defense;

        public static SpeciesData Mutate(SpeciesData main)
        {
            SpeciesData s = default;
            s.id = main.id;

            s.hp = main.hp * (1f + (0.2f * Helper.RandomValue()));
            s.saturation = main.saturation * (1f + (0.2f * Helper.RandomValue()));
            s.speed = main.speed * (1f + (0.2f * Helper.RandomValue()));
            s.power = main.power * (1f + (0.2f * Helper.RandomValue()));
            s.defense = main.defense * (1f + (0.2f * Helper.RandomValue()));
            return s;
        }
    }
}
