using UnityEngine;

public class GoodInfo : MonoBehaviour
{
    [SerializeField]
    protected string _goodName;
    [SerializeField]
    protected float _price;

    public bool IsScaned { get; set; }

    public string GoodName => _goodName;
    public float Price => _price;
}
