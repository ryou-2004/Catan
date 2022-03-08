using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DiceRoll : MonoBehaviour
{
    public Camera diceCamera;
    public GameObject spawnPoint;
    public GameObject diceRoll;

    private void Start()
    {
        diceCamera.gameObject.SetActive(false);
        diceRoll.gameObject.SetActive(false);
    }
    public void RollClick()
    {
        Dice.Clear();
        Dice.Roll("1d6", "d6-red", spawnPoint.transform.position, Force());
        Dice.Roll("1d6", "d6-red", spawnPoint.transform.position, Force());
    }
    public async UniTask<int> Roll()
    {
        Dice.Clear();
        Dice.Roll("1d6", "d6-red", spawnPoint.transform.position, Force());
        Dice.Roll("1d6", "d6-red", spawnPoint.transform.position, Force());
        await UniTask.WaitUntil(() => Dice.Value("") != 0);
        return Dice.Value("");
    }
    private Vector3 Force()
    {
        Vector3 rollTarget = Vector3.zero + new Vector3(2 + 7 * Random.value, .5F + 4 * Random.value, -2 - 3 * Random.value);
        return Vector3.Lerp(spawnPoint.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 20);
    }
}
