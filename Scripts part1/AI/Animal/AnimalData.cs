using NaughtyAttributes;
using UnityEngine;

namespace Animal
{
    [CreateAssetMenu(menuName = "Anil/Animals/AnimalData")]
    public class AnimalData : ScriptableObject
    {
        public AnimalType animalType = AnimalType.Normal;
        public MoveMode MoveMode = MoveMode.Wondering;

        public string Name = "Aminal";
        public float WalkSpeed = 5;
        public float ChaseSpeed = 10;
        public float IdleTime = 20;

        public float DetectDistance = 100;
        [ShowIf(nameof(animalType), AnimalType.Dangerous)]
        public float AttackSpeed = 2;
        [ShowIf(nameof(animalType), AnimalType.Dangerous)]
        public float AttackDistance = 25;
        [ShowIf(nameof(animalType), AnimalType.Dangerous)]
        public float Damage =  15;
    }

    public enum MoveMode
    {
        Constant, // ayı
        Wondering // neredeyse tüm heyvanlağr
    }

    public enum AnimalType
    { 
        Normal, Dangerous
    }

}