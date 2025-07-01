using System.Collections.Generic;

namespace TerraFirmaGregCalculator.Data.TreeData
{
    public class MoveListObject
    {
        public List<Node> MoveList { get; set; }
        public int MoveCount => MoveList.Count;


        public MoveListObject(Node node)
        {
            MoveList = [node];
        }

        public void AddMove(Node nodeToAdd)
        {
            MoveList.Add(nodeToAdd);
        }
    }
}
