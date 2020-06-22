﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IByteBuffer
{
    void WriteByte(byte v);
    void WriteInt(int v);
    void WriteShort(ushort v);
    void WriteLong(long v);
    void WriteFloat(float v);
    void WriteDouble(double v);
    void WriteString(string v);
    void WriteBytes(byte[] v);
    byte ReadByte();
    int ReadInt();
    ushort ReadShort();
    long ReadLong();
    float ReadFloat();
    double ReadDouble();
    string ReadString();
    byte[] ReadBytes();
    byte[] ToBytes();
    void Flush();
}

public class ByteBuffer:IByteBuffer
{
    private MemoryStream stream = null;
    private BinaryWriter writer = null;
    private BinaryReader reader = null;

    public ByteBuffer()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    public ByteBuffer(byte[] data)
    {
        if (data != null)
        {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream);
        }
        else
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }
    }

    public void Close()
    {
        writer?.Close();
        reader?.Close();
        stream.Close();
        writer = null;
        reader = null;
        stream = null;
    }

    public void WriteByte(byte v)
    {
        writer.Write(v);
    }

    public void WriteInt(int v)
    {
        writer.Write(v);
    }

    public void WriteShort(ushort v)
    {
        writer.Write((ushort)v);
    }

    public void WriteLong(long v)
    {
        writer.Write((long)v);
    }

    public void WriteFloat(float v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToSingle(temp,0));
    }

    public void WriteDouble(double v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToDouble(temp, 0));
    }

    public void WriteString(string v)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(v);
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
    }

    public void WriteBytes(byte[] v)
    {
        writer.Write((int)v.Length);
        writer.Write(v);
    }

    public byte ReadByte()
    {
        return reader.ReadByte();
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }

    public ushort ReadShort()
    {
        return (ushort)reader.ReadInt16();
    }

    public long ReadLong()
    {
        return (long)reader.ReadInt64();
    }

    public float ReadFloat()
    {
        byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
        Array.Reverse(temp);
        return BitConverter.ToSingle(temp, 0);
    }

    public double ReadDouble()
    {
        byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
        Array.Reverse(temp);
        return BitConverter.ToDouble(temp, 0);
    }

    public string ReadString()
    {
        ushort len = ReadShort();
        byte[] buffer = new byte[len];
        buffer = reader.ReadBytes(len);
        return Encoding.UTF8.GetString(buffer);
    }

    public byte[] ReadBytes()
    {
        int len = ReadInt();
        return reader.ReadBytes(len);
    }

    public byte[] ToBytes()
    {
        writer.Flush();
        return stream.ToArray();
    }

    public void Flush()
    {
        writer.Flush();
    }
}
