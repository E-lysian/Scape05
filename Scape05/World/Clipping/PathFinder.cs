using Scape05.Entities;
using Scape05.Entities.Packets;

namespace Scape05.World.Clipping;

public class PathFinder
{
    private static readonly PathFinder pathFinder = new();

    public static PathFinder getPathFinder()
    {
        return pathFinder;
    }

    public List<Location> FindRoute(Player player, int destX, int destY, bool moveNear, int xLength, int yLength)
    {
        if (destX == player.BuildArea.GetPositionRelativeToOffsetChunkX() &&
            destY == player.BuildArea.GetPositionRelativeToOffsetChunkY() && !moveNear)
        {
            PacketBuilder.SendMessage("ERROR!", player);
            return null;
        }

        destX = destX - 8 * player.BuildArea.OffsetChunkX;
        destY = destY - 8 * player.BuildArea.OffsetChunkY;

        var via = new int[104][];
        var cost = new int[104][];

        for (var i = 0; i < 104; i++)
        {
            via[i] = new int[104];
            cost[i] = new int[104];
        }

        var tileQueueX = new List<int>();
        var tileQueueY = new List<int>();

        for (var xx = 0; xx < 104; xx++)
        for (var yy = 0; yy < 104; yy++)
            cost[xx][yy] = 99999999;

        var curX = player.BuildArea.GetPositionRelativeToOffsetChunkX();
        var curY = player.BuildArea.GetPositionRelativeToOffsetChunkY();

        via[curX][curY] = 99;
        cost[curX][curY] = 0;
        var tail = 0;

        tileQueueX.Add(curX);
        tileQueueY.Add(curY);

        var foundPath = false;
        var pathLength = 4000;

        while (tail != tileQueueX.Count && tileQueueX.Count < pathLength)
        {
            curX = tileQueueX.ElementAt(tail);
            curY = tileQueueY.ElementAt(tail);


            /* Are these the correct value? */
            var curAbsX = player.BuildArea.OffsetChunkX * 8 + curX;
            var curAbsY = player.BuildArea.OffsetChunkY * 8 + curY;

            if (curX == destX && curY == destY)
            {
                foundPath = true;
                break;
            }

            tail = (tail + 1) % pathLength;
            var thisCost = cost[curX][curY] + 1;

            if (curY > 0 && via[curX][curY - 1] == 0 &&
                (Region.GetClipping(curAbsX, curAbsY - 1, player.Location.Height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY - 1);
                via[curX][curY - 1] = 1;
                cost[curX][curY - 1] = thisCost;
            }

            if (curX > 0
                && via[curX - 1][curY] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, player.Location.Height) & 0x1280108) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY);
                via[curX - 1][curY] = 2;
                cost[curX - 1][curY] = thisCost;
            }

            if (curY < 104 - 1
                && via[curX][curY + 1] == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, player.Location.Height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY + 1);
                via[curX][curY + 1] = 4;
                cost[curX][curY + 1] = thisCost;
            }

            if (curX < 104 - 1 && via[curX + 1][curY] == 0 &&
                (Region.GetClipping(curAbsX + 1, curAbsY, player.Location.Height) & 0x1280180) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY);
                via[curX + 1][curY] = 8;
                cost[curX + 1][curY] = thisCost;
            }

            if (curX > 0
                && curY > 0
                && via[curX - 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY - 1, player.Location.Height) & 0x128010e) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, player.Location.Height) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, player.Location.Height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY - 1);
                via[curX - 1][curY - 1] = 3;
                cost[curX - 1][curY - 1] = thisCost;
            }

            if (curX > 0
                && curY < 104 - 1
                && via[curX - 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY + 1, player.Location.Height) & 0x1280138) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, player.Location.Height) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, player.Location.Height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY + 1);
                via[curX - 1][curY + 1] = 6;
                cost[curX - 1][curY + 1] = thisCost;
            }

            if (curX < 104 - 1
                && curY > 0
                && via[curX + 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY - 1, player.Location.Height) & 0x1280183) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, player.Location.Height) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, player.Location.Height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY - 1);
                via[curX + 1][curY - 1] = 9;
                cost[curX + 1][curY - 1] = thisCost;
            }

            if (curX < 104 - 1
                && curY < 104 - 1
                && via[curX + 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY + 1, player.Location.Height) & 0x12801e0) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, player.Location.Height) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, player.Location.Height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY + 1);
                via[curX + 1][curY + 1] = 12;
                cost[curX + 1][curY + 1] = thisCost;
            }
        }

        if (!foundPath)
        {
            if (moveNear)
            {
                var i_223_ = 1000;
                var thisCost = 100;
                var i_225_ = 10;
                for (var x = destX - i_225_; x <= destX + i_225_; x++)
                for (var y = destY - i_225_; y <= destY + i_225_; y++)
                    if (x >= 0 && y >= 0 && x < 104 && y < 104 && cost[x][y] < 100)
                    {
                        var i_228_ = 0;
                        if (x < destX)
                            i_228_ = destX - x;
                        else if (x > destX + xLength - 1) i_228_ = x - (destX + xLength - 1);

                        var i_229_ = 0;
                        if (y < destY)
                            i_229_ = destY - y;
                        else if (y > destY + yLength - 1) i_229_ = y - (destY + yLength - 1);

                        var i_230_ = i_228_ * i_228_ + i_229_ * i_229_;
                        if (i_230_ < i_223_ || (i_230_ == i_223_ && cost[x][y] < thisCost))
                        {
                            i_223_ = i_230_;
                            thisCost = cost[x][y];
                            curX = x;
                            curY = y;
                        }
                    }

                if (i_223_ == 1000) return null;
            }
            else
            {
                return null;
            }
        }

        tail = 0;
        tileQueueX[tail] = curX;
        tileQueueY[tail++] = curY;

        int l5;
        for (var j5 = l5 = via[curX][curY];
             curX != player.BuildArea.GetPositionRelativeToOffsetChunkX() ||
             curY != player.BuildArea.GetPositionRelativeToOffsetChunkY();
             j5 = via[curX][curY])
        {
            if (j5 != l5)
            {
                l5 = j5;
                tileQueueX[tail] = curX;
                tileQueueY[tail++] = curY;
            }

            if ((j5 & 2) != 0)
                curX++;
            else if ((j5 & 8) != 0) curX--;

            if ((j5 & 1) != 0)
                curY++;
            else if ((j5 & 4) != 0) curY--;
        }

        var size = tail--;
        //Console.WriteLine(size);

        var tiles = new List<Location>();

        var pathX = player.BuildArea.OffsetChunkX * 8 + tileQueueX[tail];
        var pathY = player.BuildArea.OffsetChunkY * 8 + tileQueueY[tail];
        // Console.WriteLine($"X: {pathX} - Y: {pathY}");

        tiles.Add(new Location(pathX, pathY));
        for (var i = 1; i < size; i++)
        {
            tail--;
            pathX = player.BuildArea.OffsetChunkX * 8 + tileQueueX[tail];
            pathY = player.BuildArea.OffsetChunkY * 8 + tileQueueY[tail];
            // Console.WriteLine($"X: {pathX} - Y: {pathY}");
            tiles.Add(new Location(pathX, pathY));
        }

        player.MovementHandler.Finish();

        return tiles;
    }


    public static bool isProjectilePathClear(int x0, int y0, int z, int x1, int y1)
    {
        var deltaX = x1 - x0;
        var deltaY = y1 - y0;

        double error = 0;
        var deltaError = Math.Abs(
            deltaY / (deltaX == 0 ? deltaY : (double)deltaX));

        var x = x0;
        var y = y0;

        var pX = x;
        var pY = y;

        var incrX = x0 < x1;
        var incrY = y0 < y1;

        while (true)
        {
            if (x != x1) x += incrX ? 1 : -1;

            if (y != y1)
            {
                error += deltaError;

                if (error >= 0.5)
                {
                    y += incrY ? 1 : -1;
                    error -= 1;
                }
            }

            if (!shootable(x, y, z, pX, pY)) return false;

            if (incrX && incrY
                      && x >= x1 && y >= y1)
                break;
            if (!incrX && !incrY
                       && x <= x1 && y <= y1)
                break;
            if (!incrX && incrY
                       && x <= x1 && y >= y1)
                break;
            if (incrX && !incrY
                      && x >= x1 && y <= y1)
                break;

            pX = x;
            pY = y;
        }

        return true;
    }

    private static bool shootable(int x, int y, int z, int px, int py)
    {
        if (x == px && y == py) return true;

        var delta1 = Location.Delta(new Location(x, y), new Location(px, py));
        var delta2 = Location.Delta(new Location(px, py), new Location(x, y));

        var dir = MovementHelper.DirectionFromDelta(delta1.X, delta1.Y);
        var dir2 = MovementHelper.DirectionFromDelta(delta2.X, delta2.Y);

        if (dir == -1 || dir2 == -1) return false;

        return (Region.CanMove(x, y, z, dir) && Region.CanMove(px, py, z, dir2))
               || (Region.CanShoot(x, y, z, dir) && Region.CanShoot(px, py, z, dir2));
    }
}