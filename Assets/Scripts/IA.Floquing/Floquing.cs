using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IA.Floquing
{
    /// <summary>
    /// Extensiones de la clase Transform y Vector3 que permite calcular vectores de Floquing.
    /// </summary>
    public static class Floquing
    {
        #region Cohetion

        /// <summary>
        /// Calcula el vector Cohesion.
        /// </summary>
        /// <param name="Origin">Posicion del objeto de origen.</param>
        /// <param name="Destiny">Posicion del objeto de destino.</param>
        /// <returns>Vector3.Coh</returns>
        public static Vector3 getCohesion(this Transform Origin, Vector3 Destiny, float Magnitude = 1f)
        {
            return ((Destiny - Origin.position).normalized) * Magnitude;
        }
        /// <summary>
        /// Calcula el vector Cohesion.
        /// </summary>
        /// <param name="Origin">Transform del objeto de origen.</param>
        /// <param name="Towards">Lista de Transform´s de los objetivos.</param>
        /// <returns>Vector3.Coh</returns>
        public static Vector3 getCohesion(this Transform Origin, IEnumerable<Transform> Towards, float Magnitude = 1f)
        {
            if (!Towards.Any()) return Vector3.zero;

            Vector3 coh = Vector3.zero;
            int Targets = 0;

            foreach (var Destiny in Towards)
            {
                coh += Destiny.position - Origin.position;
                Targets++;
            }

            return ((coh / Targets).normalized) * Magnitude;
        }

        #endregion

        #region Separation

        /// <summary>
        /// Calcula el vector Separation.
        /// </summary>
        /// <param name="Origin">Objeto de origen.</param>
        /// <param name="Target">Objeto "Amigo".</param>
        /// <param name="DesiredSeparation">Distancia de separacion deseada.</param>
        /// <returns>Vector3.Sep</returns>
        public static Vector3 getSeparation(this Transform Origin, Vector3 Target, float DesiredSeparation = 1f)
        {
            Vector3 Sep = Origin.position - Target;
            float mag = DesiredSeparation - Sep.magnitude;
            return Sep.normalized * mag;
        }
        /// <summary>
        /// Calcula el vector Separation.
        /// </summary>
        /// <param name="Origin">Objeto de origen.</param>
        /// <param name="Towards">Lista de Transform´s de los objetos "Amigos".</param>
        /// <param name="DesiredSep">Distancia de separacion deseada.</param>
        /// <returns>Vector3.Sep</returns>
        public static Vector3 getSeparation(this Transform Origin, IEnumerable<Transform> Towards, float DesiredSep = 1f)
        {
            if (!Towards.Any()) return Vector3.zero;

            Vector3 Sep = Vector3.zero;
            int targets = 0;

            foreach (Transform Target in Towards)
            {
                Vector3 ItemSep = Origin.position - Target.position;
                float mag = DesiredSep - ItemSep.magnitude;

                Sep += ItemSep.normalized * mag;
                targets++;
            }

            return (Sep / targets);
        }

        #endregion

        #region Alignment

        /// <summary>
        /// Calcula el vector Alignment.
        /// </summary>
        /// <param name="Origin">Objeto de origen.</param>
        /// <param name="Towards">Lista de "amigos".</param>
        /// <returns>Vector3.Alig</returns>
        public static Vector3 getAlignment(this Transform Origin, IEnumerable<Transform> Towards)
        {
            Vector3 Alig = Vector3.zero;
            int targets = 0;

            foreach (Transform Target in Towards)
            {
                Alig += Target.forward;
                targets++;
            }

            return (Alig / targets).normalized;
        }
        #endregion

        #region Avoidance

        /// <summary>
        /// Calcula el vector Avoidance. Este vector es el opuesto al vector que va desde el origen al objetivo.
        /// </summary>
        /// <param name="Origin">Vector posicion de origen del objeto A.</param>
        /// <param name="Target">Vector posicion de origen del objeto B.</param>
        /// <param name="normalized">¿Se quiere el resultado normalizado?</param>
        /// <returns>Retorna el vector de Avoidance normalizado</returns>
        public static Vector3 getAvoidance(this Vector3 Origin, Vector3 Target, bool normalized = true)
        {
            return ( normalized ? (Origin - Target).normalized : (Origin - Target));
        }
        /// <summary>
        /// Calcula el vector Avoidance. Este vector es el opuesto al vector que va desde el origen al objetivo.
        /// </summary>
        /// <param name="Origin">Vector posicion de origen del objeto A.</param>
        /// <param name="Target">Vector posicion de origen del objeto B.</param>
        /// <param name="normalized">¿Se quiere el resultado normalizado?</param>
        /// <returns>Retorna el vector de Avoidance normalizado</returns>
        public static Vector3 getAvoidance(this Transform Origin, Transform Target, bool normalized = true)
        {
            return (normalized ? (Origin.position - Target.position).normalized : (Origin.position - Target.position));
        }
        /// <summary>
        /// Retorna el vector resultante de todos los vectores Avoidance.
        /// </summary>
        /// <param name="Origin">Vector de posición origen</param>
        /// <param name="Towards">Collección de posiciones de objetos a evadir</param>
        /// <returns>El vector Avoidance sin normalizar</returns>
        public static Vector3 getAvoidance(this Transform Origin, IEnumerable<Transform> Towards, bool normalized = false, float magnitudeModifier = 1f, float Scale = 1f)
        {
            if (!Towards.Any()) return Vector3.zero;

            Vector3 avoid = Vector3.zero;
            int targets = 0;

            foreach (Transform Target in Towards)
            {
                avoid += Origin.getAvoidance(Target, normalized);
                targets++;
            }

            return (avoid / targets) * Scale;
        }

        /// <summary>
        /// Retorna un vector de avoidance Pesado!. El peso se calcula en base a la distancia del obstaculo, cuando mas cercano mayor es su peso, se puede escalar el vector!.
        /// </summary>
        /// <param name="Origin">origen del Objeto.</param>
        /// <param name="Obstacles">Lista de Objetos "Obstáculos"</param>
        /// <param name="maxAvoidanceRadius"></param>
        /// <param name="minAvoidanceRadious"></param>
        /// <param name="magnitudeModifier"></param>
        /// <param name="normalized"></param>
        /// <param name="Weight"></param>
        /// <returns>A weighted Avoidance Vector</returns>
        public static Vector3 getAvoidance(this Transform Origin, IEnumerable<Transform> Obstacles, float maxAvoidanceRadius, float minAvoidanceRadious = 0, bool normalized = false, float magnitudeModifier = 1f, float Weight = 1f)
        {
            if (!Obstacles.Any()) return Vector3.zero;

            Vector3 avoid = Vector3.zero;       //
            int targets = 0;                    //Cantidad de objetivos.
            Vector3 vecToTarget = Vector3.zero; //Acumulador.

            foreach (Transform Target in Obstacles)
            {
                vecToTarget = Origin.position - Target.position;
                float distToTarget = vecToTarget.magnitude;

                //Formula es D-R/R, donde: D = distancia, R = radio.
                float strenght = (distToTarget - maxAvoidanceRadius) / maxAvoidanceRadius;
                Debug.Log("strenght is: " + strenght);

                avoid += ((vecToTarget.normalized).ScaleTo(Weight) * (strenght * Weight));
                targets++;
            }

            return (avoid / targets);  //No escalo el resultado, el peso lo aplicamos a nivel individual.
        }

        public static Vector3 getAvoidance(this Transform Origin, IEnumerable<Transform> Obstacles, Func<float,float> MagnitudeModifier, float maxAvoidanceRadius, float minAvoidanceRadious = 0, bool avereage = false)
        {
            if (!Obstacles.Any()) return Vector3.zero;

            Vector3 avoid = Vector3.zero;       //Acumulador.
            int targets = 0;                    //Cantidad de objetivos.

            foreach (Transform Target in Obstacles)
            {
                Vector3 vecToTarget = Origin.position - Target.position;
                float distToTarget = vecToTarget.magnitude;
                //Strenght es un numero que va de 0 a 1 y es un valor lerpeado.
                
                float weight = 1f;

                if (distToTarget < minAvoidanceRadious) //Si esta dentro del radio mínimo
                    weight = 1f;
                else if (distToTarget > maxAvoidanceRadius) //Si está fuera del radio máximo.
                    weight = 0f;
                else
                {
                    //Modificación de la fórmula donde: D-Rm/RM - Rm, donde: D = distancia, Rm = radio menor, RM radio mayor.
                    float strenght = (distToTarget - minAvoidanceRadious) / (maxAvoidanceRadius - minAvoidanceRadious);
                    //Debug.Log("strenght is: " + strenght);

                    //Magnitude modifier es un postprocesado al valor del peso.
                    weight = MagnitudeModifier(strenght);
                }

                avoid += ((vecToTarget.normalized) * weight);
                targets++;
            }

            return avereage ? (avoid / targets) : avoid;
        }

        #endregion
    }
}

