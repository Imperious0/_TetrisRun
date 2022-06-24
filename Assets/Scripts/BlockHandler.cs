using UnityEngine;

public class BlockHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject block;

    private BlockMechanics b_Mechanics;

    [SerializeField]
    private float widthOfBlock = 3f;

    public float WidthOfBlock { get => widthOfBlock; }
    private void Awake()
    {
        b_Mechanics = block.GetComponent<BlockMechanics>();
    }
    public void setMyTurn(bool isMyTurn) 
    {
        b_Mechanics.setTurn(isMyTurn);
    }
    public void resetBlock() 
    {
        b_Mechanics.resetBlock();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) 
        {
            if (b_Mechanics.isBlocksKiller()) 
            {
                if(b_Mechanics.isNaturalDeath())
                    b_Mechanics.dieTie();
            }

        }
    }
}
