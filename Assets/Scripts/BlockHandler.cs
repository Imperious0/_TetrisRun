using UnityEngine;

public class BlockHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject block;

    [SerializeField]
    private float widthOfBlock = 3f;

    public float WidthOfBlock { get => widthOfBlock; }

    public void setMyTurn(bool isMyTurn) 
    {
        block.GetComponent<BlockMechanics>().setTurn(isMyTurn);
    }
    public void resetBlock() 
    {
        block.GetComponent<BlockMechanics>().resetBlock();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) 
        {
            if (block.GetComponent<BlockMechanics>().isBlocksKiller()) 
            {
                if(block.GetComponent<BlockMechanics>().isNaturalDeath())
                    block.GetComponent<BlockMechanics>().dieTie();
            }

        }
    }
}
