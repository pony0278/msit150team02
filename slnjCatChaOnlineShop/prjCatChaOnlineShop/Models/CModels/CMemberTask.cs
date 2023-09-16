namespace prjCatChaOnlineShop.Models.CModels
{
    public class CMemberTask
    {
        public int fMemberTaskId { get; set; }

        public int? fMemberId { get; set; }

        public int? fTaskId { get; set; }

        public int? fTaskProgress { get; set; }

        public DateTime? fCompleteDate { get; set; }


    }
}
