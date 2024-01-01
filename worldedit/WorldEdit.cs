using MiNET;
using MiNET.Blocks;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils.Vectors;

namespace worldedit
{
    [Plugin(PluginName = "WorldEdit", Description = "Basic world editor for MiNET", PluginVersion = "1.0.0", Author = "CobwebSMP")]
    public class WorldEdit : Plugin
    {
        private static int count = 0;
        private static int count2 = 0;

        private static int xv = 0;
        private static int yv = 0;
        private static int zv = 0;

        private static PlayerLocation pos1v = new PlayerLocation(0, 0, 0);
        private static PlayerLocation pos2v = new PlayerLocation(0, 0, 0);

        private static int[] blocks = new int[100000];

        [Command(Name = "pos1", Description = "Set pos1 to your current location.")]
        public void pos1(Player player)
        {
            pos1v = player.KnownPosition;
            player.SendMessage($"§eWorldEdit: §fPOS1 set to {(int)pos1v.X} {(int)pos1v.Y} {(int)pos1v.Z}.");
        }

        [Command(Name = "pos2", Description = "Set pos2 to your current location.")]
        public void pos2(Player player)
        {
            pos2v = player.KnownPosition;
            player.SendMessage($"§eWorldEdit: §fPOS2 set to {(int)pos2v.X} {(int)pos2v.Y} {(int)pos2v.Z}.");
        }

        [Command(Name = "copy", Description = "Copy arena. Arena cube corner 1 pos<x y z> corner 2 pos <x y z>")]
        public void copy(Player player, int x1, int y1, int z1, int x2, int y2, int z2)
        {
             xv = Math.Max(x1, x2) - Math.Min(x1, x2);
             yv = Math.Max(y1, y2) - Math.Min(y1, y2);
             zv = Math.Max(z1, z2) - Math.Min(z1, z2);

             var level = player.Level;
             for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
             {
                 for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                 {
                     for (int z = Math.Min(z1, z2); z <= Math.Max(z1, z2); z++)
                     {
                        var block = level.GetBlock(new BlockCoordinates(x, y, z));
                        blocks[count] = block.GetRuntimeId();
                        count++;
                        if (count == 100000)
                        {
                            player.SendMessage($"§eWorldEdit: §fCan't save more than 100 000 blocks. Try to copy smaller arena.");
                            break;
                        }
                     }
                 }
             }
             player.SendMessage($"§eWorldEdit: §fSaved {count} blocks to memory. Use paste to paste blocks to your current location.");
             count = 0;
        }

        [Command(Name = "copy2", Description = "Copy arena using preset Pos1 and Pos2")]
        public void copy2(Player player)
        {
            if (pos1v.X == pos2v.X || pos1v.Y == pos2v.Y || pos1v.Z == pos2v.Z)
            {
                player.SendMessage($"§eWorldEdit: §fPos1 and/or Pos2 not set.");
                return;
            }
            xv = Math.Max((int)pos1v.X, (int)pos2v.X) - Math.Min((int)pos1v.X, (int)pos2v.X);
            yv = Math.Max((int)pos1v.Y, (int)pos2v.Y) - Math.Min((int)pos1v.Y, (int)pos2v.Y);
            zv = Math.Max((int)pos1v.Z, (int)pos2v.Z) - Math.Min((int)pos1v.Z, (int)pos2v.Z);

            var level = player.Level;
            for (int x = Math.Min((int)pos1v.X, (int)pos2v.X); x <= Math.Max((int)pos1v.X, (int)pos2v.X); x++)
            {
                for (int y = Math.Min((int)pos1v.Y, (int)pos2v.Y); y <= Math.Max((int)pos1v.Y, (int)pos2v.Y); y++)
                {
                    for (int z = Math.Min((int)pos1v.Z, (int)pos2v.Z); z <= Math.Max((int)pos1v.Z, (int)pos2v.Z); z++)
                    {
                        var block = level.GetBlock(new BlockCoordinates(x, y, z));
                        blocks[count] = block.GetRuntimeId();
                        count++;
                        if (count == 100000)
                        {
                            player.SendMessage($"§eWorldEdit: §fCan't save more than 100 000 blocks. Try to copy smaller arena.");
                            break;
                        }
                    }
                }
            }
            player.SendMessage($"§eWorldEdit: §fSaved {count} blocks to memory. Use /paste to paste blocks to your current location.");
            count = 0;
        }

        [Command(Name = "paste", Description = "Paste most recent saved blocks (using /copy) to your current location.")]
        public async void paste(Player player)
        {
            var level = player.Level;
            var pos = player.KnownPosition;
            for (int x = 0; x <= xv; x++)
            {
                for (int y = 0; y <= yv; y++)
                {
                    for (int z = 0; z <= zv; z++)
                    {
                        await Task.Run(() =>
                        {
                            Block block = BlockFactory.GetBlockByRuntimeId(blocks[count2]);
                            block.Coordinates = (new BlockCoordinates((int)pos.X + x, (int)pos.Y + y, (int)pos.Z + z));
                            level.SetBlock(block);
                            count2++;
                        });
                    }
                }
            }
            player.SendMessage($"§eWorldEdit: §fPlaced {count2} blocks.");
            count2 = 0;
        }
    }
}