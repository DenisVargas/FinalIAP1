using System.Collections.Generic;
using UnityEngine;

//Buscar una manera de que el Line of SightComponent soporte varios rangos!

namespace IA.LineOfSight
{
    public class LineOfSightComponent : MonoBehaviour
    {
        public LayerMask visibles = ~0;
        public float range = 1f;
        public float angle = 45f;

#region DEBUG
#if UNITY_EDITOR
        [Space, Header("Debug")]
        [SerializeField] bool Debug_LineOfSight = false; 
        [SerializeField] Color rangeColor = Color.white;
        [SerializeField] Color angleColor = Color.white;

        void OnDrawGizmosSelected()
        {
            if (Debug_LineOfSight)
            {
                //Posición del objetivo.
                var currentPosition = transform.position;

                //Rango
                Gizmos.color = rangeColor;
                Gizmos.matrix *= Matrix4x4.Scale(new Vector3(1, 0, 1));
                Gizmos.DrawWireSphere(transform.position, range);

                //Ángulo
                Gizmos.color = angleColor;
                Gizmos.DrawLine(currentPosition, currentPosition + Quaternion.Euler(0, angle + 1, 0) * transform.forward * range);
                Gizmos.DrawLine(currentPosition, currentPosition + Quaternion.Euler(0, -angle - 1, 0) * transform.forward * range);
            }

        }
#endif
#endregion

        /// <summary>
        /// El vector resultante de la resta de ambas posiciones: B - A.
        /// </summary>
        [ HideInInspector]
        public Vector3 positionDiference = Vector3.zero;
        /// <summary>
        /// Dirección normalizada hacia el objetivo.
        /// </summary>
        [ HideInInspector]
        public Vector3 dirToTarget = Vector3.zero;
        /// <summary>
        /// Último ángulo calculado entre la posición de origen y el objetivo.
        /// </summary>
        [HideInInspector]
        public float angleToTarget = 0;
        /// <summary>
        /// Última distancia calculada entre la posición de origen y el objetivo.
        /// </summary>
        [HideInInspector]
        public float distanceToTarget = 0;

        /// <summary>
        /// Indica si el objetivo específicado está dentro de la línea de visión
        /// </summary>
        /// <param name="target">Objetivo a comprobar</param>
        /// <returns>Verdadero si el Objetivo específicado está dentro de la línea de visión</returns>
        public bool IsInSight(Transform target)
        {
            if (target == null)
            {
                Debug.Log("El target es inválido");
                return false;
            }

            positionDiference = (target.position - transform.position);
            distanceToTarget = positionDiference.magnitude;
            Vector3 BidimensionalProjection = positionDiference.YComponent(0);
            angleToTarget = Vector3.Angle(transform.forward, BidimensionalProjection);
            dirToTarget = positionDiference.normalized;

            dirToTarget = positionDiference.normalized;

            if (distanceToTarget > range || angleToTarget > angle) return false;

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, dirToTarget, out hitInfo, range + 1, visibles))
                return hitInfo.transform == target;

            return false;
        }
        /// <summary>
        /// Indica si el objetivo específicado está dentro de la línea de visión
        /// </summary>
        /// <param name="target">Objetivo a comprobar</param>
        /// <param name="range">Rango máximo de visión</param>
        /// <param name="angle">Ángulo máximo de visión</param>
        /// <param name="visibles">Filtro para los objetos visibles</param>
        /// <returns>Verdadero si el Objetivo específicado está dentro de la línea de visión</returns>
        public bool IsInSight(Transform target, float range, float angle, LayerMask visibles)
        {
            if (target == null)
            {
                Debug.Log("El target es inválido");
                return false;
            }

            positionDiference = (target.position - transform.position);
            distanceToTarget = positionDiference.magnitude;
            Vector3 BidimensionalProjection = positionDiference.YComponent(0);
            angleToTarget = Vector3.Angle(transform.forward, BidimensionalProjection);
            dirToTarget = positionDiference.normalized;

            dirToTarget = positionDiference.normalized;

            if (distanceToTarget > range || angleToTarget > angle) return false;

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, dirToTarget, out hitInfo, range + 1, visibles))
                return hitInfo.transform == target;

            return false;
        }

        public List<Transform> GetAllVisibles(Vector3 origin, LayerMask visibles)
        {
            List<Transform> visibleAgents = new List<Transform>();
            Collider[] closerEntities = Physics.OverlapSphere(origin, range, visibles); //chequeo mi entorno.

            foreach (var item in closerEntities)
                if (IsInSight(item.transform))
                    visibleAgents.Add(item.transform);

            return visibleAgents;
        }
        public List<Transform> GetAllVisibles(Vector3 origin, float range, float angle, LayerMask visibles)
        {
            List<Transform> visibleAgents = new List<Transform>();
            Collider[] closerEntities = Physics.OverlapSphere(origin, range, visibles); //chequeo mi entorno.

            foreach (var item in closerEntities)
                if (IsInSight(item.transform, range, angle, visibles))
                    visibleAgents.Add(item.transform);

            return visibleAgents;
        }
    }
}

