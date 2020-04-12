using Commodore.EVIL;
using System.Collections.Generic;

namespace Commodore.GameLogic.Core.Hardware
{
    public class Memory : IMemory
    {
        public List<byte[]> Planes { get; }
        public byte CurrentPlane { get; private set; }

        public byte[] Array => Planes[CurrentPlane];

        public Memory(uint size)
        {
            Planes = new List<byte[]>
            {
                new byte[size]
            };

            CurrentPlane = 0;
        }

        public Memory(uint size, byte planes)
        {
            Planes = new List<byte[]>();

            if (planes == 0)
            {
                Planes.Add(new byte[size]);
            }
            else
            {
                for (var i = 0; i < planes; i++)
                {
                    Planes.Add(new byte[size]);
                }
            }

            CurrentPlane = 0;
        }

        public void SetPlane(byte plane)
        {
            CurrentPlane = plane;
        }

        public void ClearPlanesAndSetFirst()
        {
            foreach (var plane in Planes)
            {
                System.Array.Clear(plane, 0, plane.Length);
            }

            CurrentPlane = 0;
        }

        // YES I KNOW, FUTURE ME
        // THIS CODE SUCKS
        //
        // IT DIDN'T SUCK BEFORE I INTRODUCED BYTEPLANES T_T
        public void Poke(int addr, byte value) => Array[addr % Array.Length] = value;

        public void Poke(byte plane, int addr, byte value)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            Poke(addr, value);
            SetPlane(prevPlane);
        }

        public void Poke(int addr, bool value) => Poke(addr, value ? (byte)1 : (byte)0);

        public void Poke(byte plane, int addr, bool value)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            Poke(addr, value);
            SetPlane(prevPlane);
        }

        public void Poke(int addr, short value)
        {
            Array[addr % Array.Length] = (byte)(value & 0x00FF);
            Array[(addr + 1) % Array.Length] = (byte)((value & 0xFF00) >> 8);
        }

        public void Poke(byte plane, int addr, short value)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            Poke(addr, value);
            SetPlane(prevPlane);
        }

        public void Poke(int addr, int value)
        {
            Array[addr % Array.Length] = (byte)(value & 0x000000FF);
            Array[(addr + 1) % Array.Length] = (byte)((value & 0x0000FF00) >> 8);
            Array[(addr + 2) % Array.Length] = (byte)((value & 0x00FF0000) >> 16);
            Array[(addr + 3) % Array.Length] = (byte)((value & 0xFF000000) >> 24);
        }

        public void Poke(byte plane, int addr, int value)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            Poke(addr, value);
            SetPlane(prevPlane);
        }

        public bool PeekBool(int addr) => Array[addr % Array.Length] != 0;

        public bool PeekBool(byte plane, int addr)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            var retval = PeekBool(addr);
            SetPlane(prevPlane);

            return retval;
        }

        public byte Peek8(int addr) => Array[addr % Array.Length];

        public byte Peek8(byte plane, int addr)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            var retval = Peek8(addr);
            SetPlane(prevPlane);

            return retval;
        }

        public short Peek16(int addr)
        {
            ushort ret = 0;
            ret |= Array[addr % Array.Length];
            ret |= (ushort)(Array[(addr + 1) % Array.Length] << 8);

            return (short)ret;
        }

        public short Peek16(byte plane, int addr)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            var retval = Peek16(addr);
            SetPlane(prevPlane);

            return retval;
        }

        public int Peek32(int addr)
        {
            uint ret = 0;
            ret |= Array[addr % Array.Length];
            ret |= (uint)(Array[(addr + 1) % Array.Length] << 8);
            ret |= (uint)(Array[(addr + 2) % Array.Length] << 16);
            ret |= (uint)(Array[(addr + 3) % Array.Length] << 24);

            return (int)ret;
        }

        public int Peek32(byte plane, int addr)
        {
            var prevPlane = CurrentPlane;
            SetPlane(plane);

            var retval = Peek32(addr);
            SetPlane(prevPlane);

            return retval;
        }
    }
}