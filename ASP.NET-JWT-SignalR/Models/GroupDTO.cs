namespace LlamaEngineHost.Models
{
    public class GroupDTO
    {
        public int GroupNumber { get; set; }
        public string GroupName { get; set; }

        // Static list of all groups
        public static readonly List<GroupDTO> All = new()
        {
            new GroupDTO { GroupNumber = 1, GroupName = "Room 1" },
            new GroupDTO { GroupNumber = 2, GroupName = "Room 2" },
            new GroupDTO { GroupNumber = 3, GroupName = "Room 3" }
        };
    }
}
