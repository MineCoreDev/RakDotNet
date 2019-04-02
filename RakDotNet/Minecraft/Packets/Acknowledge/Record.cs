using System;
using System.Collections.Generic;

namespace RakDotNet.Minecraft.Packets.Acknowledge
{
    public class Record
    {
        public uint StartIndex { get; set; }
        public uint EndIndex { get; set; }

        public static List<Record> SortRecords(Record[] records)
        {
            List<uint> rebuildList = new List<uint>();
            for (int i = 0; i < records.Length; i++)
            {
                Record record = records[i];
                if (!record.IsRanged())
                {
                    rebuildList.Add(record.StartIndex);
                }
                else
                    throw new InvalidOperationException("records sorted.");
            }

            rebuildList.Sort();

            List<Record> buildRecords = new List<Record>();
            uint start = 0;
            uint end = 0;
            uint current = 0;
            int index = 0;

            while (index < rebuildList.Count - 1)
            {
                start = rebuildList[index];
                for (int i = index; i < rebuildList.Count; i++)
                {
                    current = rebuildList[i];
                    if (rebuildList[i + 1] - 1 != current)
                    {
                        end = current;
                        break;
                    }
                }

                if (start == end)
                    buildRecords.Add(new Record(start));
                else
                    buildRecords.Add(new Record(start, end));
            }

            return buildRecords;
        }

        public Record(uint index)
        {
            StartIndex = index;
            EndIndex = index;
        }

        public Record(uint startIndex, uint endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public bool IsRanged()
        {
            return StartIndex != EndIndex;
        }
    }
}