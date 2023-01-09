using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;

public class DonutAnimator
{
    
    public List<Task> moveTasks = new List<Task>(); 
    public List<Task> swapTasks = new List<Task>();

    public void TransferDonut(Block blockFrom, Block blockTo)
    {
        Donut donutToSwap = blockFrom.RemoveDonut();
        blockTo.AddDonut(donutToSwap);
        swapTasks.Add(donutToSwap.transform.DOJump(blockTo.PlaceToMoveDonut().position, 0.5f,1,0.5f).AsyncWaitForCompletion());
        swapTasks.Add(blockTo.transform.DOShakeScale(1f, 0.2f).OnComplete(() => blockTo.ResetDonutsLocalPositionOrKill()).AsyncWaitForCompletion());
    }

    public void TransferTwoDonuts(Block blockFrom, Block blockTo)
    {
        Donut firstDonutToSwap = blockFrom.RemoveDonut();
        blockTo.AddDonut(firstDonutToSwap);
        swapTasks.Add(firstDonutToSwap.transform.DOJump(blockTo.PlaceToMoveDonut().position, 0.5f, 1, 0.5f)
                .OnComplete(()=>TransferDonut(blockFrom,blockTo)).AsyncWaitForCompletion());
    }
    
    public void TransferThreeDonuts(Block blockFrom,Block blockThrough, Block blockTo)
    {

        
        Donut firstDonutToSwap = blockFrom.RemoveDonut();
        blockThrough.AddDonut(firstDonutToSwap);
        
        
        swapTasks.Add(DOTween.Sequence()
            .Append(firstDonutToSwap.transform.DOJump(blockThrough.PlaceToMoveDonut().position, 0.5f, 1, 0.5f))
            .AsyncWaitForCompletion());
        
        Donut donutToSwap = blockThrough.RemoveDonut();
        blockTo.AddDonut(donutToSwap);

        swapTasks.Add(DOTween.Sequence()
            .Append(donutToSwap.transform.DOJump(blockTo.PlaceToMoveDonut().position, 0.5f, 1, 0.5f))
            .OnComplete(() => TransferDonut(blockThrough, blockTo)).AsyncWaitForCompletion());






        //swapTasks.Add(firstDonutToSwap.transform.DOJump(blockThrough.PlaceToMoveDonut().position, 0.5f, 1, 0.5f)
        //        .OnComplete(()=>TransferTwoDonuts(blockThrough, blockTo))
        //        .AsyncWaitForCompletion());
    }
    
    public void MoveBlock(Block block, Tile to ,float time)
    {
        block.MoveToNewTile(to);
        moveTasks.Add(block.transform.DOMove(to.transform.position, time)
            .SetEase(Ease.Linear).AsyncWaitForCompletion());
    }    
    
}