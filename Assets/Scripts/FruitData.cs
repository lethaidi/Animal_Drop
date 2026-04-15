using UnityEngine;

[CreateAssetMenu(fileName = "New Fruit", menuName = "Suika/Fruit Data")]
public class FruitData : ScriptableObject
{
    public int index;
    public string fruitName;
    public Sprite sprite; 
    public int scoreValue;
    public float spawnScale;
    public float radiusCollider;
}