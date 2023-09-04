namespace prjCatChaOnlineShop.Models.CModels
{
    public class CMemberTask
    {
        public int MemberTaskId { get; set; }

        public int? MemberId { get; set; }

        public int? TaskId { get; set; }

        public int? TaskProgress { get; set; }

        public DateTime? CompleteDate { get; set; }


    }
}
