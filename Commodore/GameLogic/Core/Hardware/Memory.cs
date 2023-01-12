using Commodore.EVIL;
using System.Collections.Generic;

namespace Commodore.GameLogic.Core.Hardware
{
    public class Memory : IMemory
    {
        private byte[] _memory;
        public byte[] Array => _memory;

        public Memory(uint size)
        {
            _memory = new byte[size];
        }

        public void Clear()
        {
            for (var i = 0; i < _memory.Length; i++)
                _memory[i] = 0;
        }

        public void Poke(int addr, byte value) => Array[addr % Array.Length] = value;
        public void Poke(int addr, bool value) => Poke(addr, value ? (byte)1 : (byte)0);

        public void Poke(int addr, short value)
        {
            Array[addr % Array.Length] = (byte)(value & 0x00FF);
            Array[(addr + 1) % Array.Length] = (byte)((value & 0xFF00) >> 8);
        }

        public void Poke(int addr, int value)
        {
            Array[addr % Array.Length] = (byte)(value & 0x000000FF);
            Array[(addr + 1) % Array.Length] = (byte)((value & 0x0000FF00) >> 8);
            Array[(addr + 2) % Array.Length] = (byte)((value & 0x00FF0000) >> 16);
            Array[(addr + 3) % Array.Length] = (byte)((value & 0xFF000000) >> 24);
        }


        public bool PeekBool(int addr) => Array[addr % Array.Length] != 0;

        public byte Peek8(int addr) => Array[addr % Array.Length];

        public short Peek16(int addr)
        {
            ushort ret = 0;
            ret |= Array[addr % Array.Length];
            ret |= (ushort)(Array[(addr + 1) % Array.Length] << 8);

            return (short)ret;
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
    }
}