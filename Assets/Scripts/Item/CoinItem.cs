using UnityEngine;

[System.Serializable]
public enum CoinType
{
    coin1,
    coin2,
    coin3,
    coin4,
    coin5
}

public class CoinItem : MonoBehaviour
{
    public CoinType coinType;

    [HideInInspector]
    public int value;

    void Awake()
    {
        SetValue();
    }

    void SetValue()
    {
        switch (coinType)
        {
            case CoinType.coin1:
                value = 1;
                break;
            case CoinType.coin2:
                value = 5;
                break;
            case CoinType.coin3:
                value = 10;
                break;
            case CoinType.coin4:
                value = 25;
                break;
            case CoinType.coin5:
                value = 50;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript.instance.AddGold(value);
            Destroy(gameObject);
        }
    }
}
