// Decompiled with JetBrains decompiler
// Type: ISO8583
// Assembly: ConveyISO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A1A29DB8-D4AD-4F47-B8FA-3FADEED7E861
// Assembly location: C:\Users\rodrigo.groff\Desktop\ciso\ConveyISO.exe

using System;
using System.Collections;
using System.Globalization; 

public class ISO8583
{
  private string m_registro = " ";
  private bool m_erro = false;
  private BitArray mBits1 = new BitArray(65);
  private BitArray mBits2 = new BitArray(65);
  private string m_bit22 = "";
  private string m_bit62 = "";
  private string m_bit63 = "";
  private string m_bit64 = "";
  private string m_bit120 = "";
  private string m_bit49 = "";
  private string m_bit90 = "";
  private string m_bit127 = "";
  private string m_bit125 = "";
  private string m_mapaBit1 = "";
  private string m_mapaBit2 = "";
  private string m_trilha2 = "";
  private string m_valor = "";
  private string m_terminal = "";
  private string m_codLoja = "";
  private string m_codigo;
  private string m_codProcessamento;
  private string m_PAN;
  private string m_datetime;
  private string m_time;
  private string m_date;
  private string m_dateExp;
  private string m_security;
  private string m_codResposta;
  private string m_senha;
  private string m_nsuOrigem;
  private string m_nsu;

  public string codigo
  {
    get
    {
      return this.m_codigo;
    }
    set
    {
      this.m_codigo = value.PadLeft(4, '0');
    }
  }

  public bool erro
  {
    get
    {
      return this.m_erro;
    }
    set
    {
    }
  }

  public string mapaBit1
  {
    get
    {
      this.m_mapaBit1 = "";
      int num = 0;
      int bitnum = 1;
      for (int index1 = 0; index1 < 8; ++index1)
      {
        for (int index2 = 0; index2 < 8; ++index2)
        {
          if (this.getBit(bitnum))
            num += (int) Math.Pow(2.0, (double) (7 - index2));
          ++bitnum;
        }
        this.m_mapaBit1 = this.m_mapaBit1 + string.Format("{0:X2}", (object) num);
        num = 0;
      }
      return this.m_mapaBit1;
    }
    set
    {
    }
  }

  public string mapaBit2
  {
    get
    {
      this.m_mapaBit2 = "";
      int num1 = 0;
      int num2 = 1;
      for (int index1 = 0; index1 < 8; ++index1)
      {
        for (int index2 = 0; index2 < 8; ++index2)
        {
          if (this.getBit(num2 + 64))
            num1 += (int) Math.Pow(2.0, (double) (7 - index2));
          ++num2;
        }
        this.m_mapaBit2 = this.m_mapaBit2 + string.Format("{0:X2}", (object) num1);
        num1 = 0;
      }
      return this.m_mapaBit2;
    }
    set
    {
    }
  }

  public string PAN
  {
    get
    {
      return this.m_PAN;
    }
    set
    {
      this.setBit(2);
      this.m_PAN = value.PadLeft(19, '0');
    }
  }

  public string codProcessamento
  {
    get
    {
      return this.m_codProcessamento;
    }
    set
    {
      this.setBit(3);
      this.m_codProcessamento = value.PadLeft(6, '0');
    }
  }

  public string valor
  {
    get
    {
      return this.m_valor;
    }
    set
    {
      this.setBit(4);
      this.m_valor = value.PadLeft(12, '0');
    }
  }

  public string datetime
  {
    get
    {
      return this.m_datetime;
    }
    set
    {
    }
  }

  public string Time
  {
    get
    {
      return this.m_time;
    }
    set
    {
    }
  }

  public string Date
  {
    get
    {
      return this.m_date;
    }
    set
    {
    }
  }

  public string DateExp
  {
    get
    {
      return this.m_dateExp;
    }
    set
    {
      this.setBit(14);
      this.m_dateExp = value.PadLeft(4, '0');
    }
  }

  public string bit22
  {
    get
    {
      return this.m_bit22;
    }
    set
    {
      this.setBit(22);
      this.m_bit22 = value.PadRight(3, '0');
    }
  }

  public string trilha2
  {
    get
    {
      return this.m_trilha2;
    }
    set
    {
      this.setBit(35);
      this.m_trilha2 = value;
    }
  }

  public string codResposta
  {
    get
    {
      return this.m_codResposta;
    }
    set
    {
      this.setBit(39);
      this.m_codResposta = value.PadLeft(2, '0');
    }
  }

  public string terminal
  {
    get
    {
      return this.m_terminal;
    }
    set
    {
      this.setBit(41);
      this.m_terminal = value.PadLeft(8, '0');
    }
  }

  public string codLoja
  {
    get
    {
      return this.m_codLoja;
    }
    set
    {
      this.setBit(42);
      this.m_codLoja = value.PadLeft(15, '0');
    }
  }

  public string senha
  {
    get
    {
      return this.m_senha;
    }
    set
    {
      this.setBit(52);
      this.m_senha = value.PadLeft(16, '0');
    }
  }

  public string securityControl
  {
    get
    {
      return this.m_security;
    }
    set
    {
      this.setBit(53);
      this.m_security = value.PadLeft(6, '0');
    }
  }

  public string bit62
  {
    get
    {
      if (this.m_bit62.Length > 3)
        return this.m_bit62.Substring(3);
      return this.m_bit62;
    }
    set
    {
      this.setBit(62);
      this.m_bit62 = value.Length.ToString("000") + value;
    }
  }

  public string bit63
  {
    get
    {
      if (this.m_bit63.Length > 3)
        return this.m_bit63.Substring(3);
      return this.m_bit63;
    }
    set
    {
      this.setBit(63);
      this.m_bit63 = value.Length.ToString("000") + value;
    }
  }

  public string bit64
  {
    get
    {
      if (this.m_bit64.Length > 3)
        return this.m_bit64.Substring(3);
      return this.m_bit64;
    }
    set
    {
      this.setBit(64);
      this.m_bit64 = value.Length.ToString("000") + value;
    }
  }

  public string bit49
  {
    get
    {
      return this.m_bit49;
    }
    set
    {
      this.setBit(49);
      this.m_bit49 = value.PadLeft(3, '0');
    }
  }

  public string bit90
  {
    get
    {
      return this.m_bit90;
    }
    set
    {
      this.setBit(90);
      this.m_bit90 = value.PadRight(42, '0');
    }
  }

  public string bit120
  {
    get
    {
      if (this.m_bit120.Length > 3)
        return this.m_bit120.Substring(3);
      return this.m_bit120;
    }
    set
    {
      this.setBit(120);
      this.m_bit120 = value.Length.ToString("000") + value;
    }
  }

  public string registro
  {
    get
    {
      return this.monta_registro();
    }
    set
    {
      this.m_registro = value;
      this.desmonta_registro(value);
    }
  }

  public string relacaoBits
  {
    get
    {
      string str = "";
      for (int bitnum = 0; bitnum < 128; ++bitnum)
      {
        if (this.getBit(bitnum))
          str = str + bitnum.ToString() + ",";
      }
      if (str.Length > 0)
        str = str.Substring(0, str.Length - 1);
      return str;
    }
    set
    {
    }
  }

  public string bit125
  {
    get
    {
      return this.m_bit125;
    }
    set
    {
      this.setBit(125);
      this.m_bit125 = value;
    }
  }

  public string bit127
  {
    get
    {
      return this.m_bit127;
    }
    set
    {
      this.setBit((int) sbyte.MaxValue);
      this.m_bit127 = value;
    }
  }

  public string nsuOrigem
  {
    get
    {
      return this.m_nsuOrigem;
    }
    set
    {
      this.setBit(11);
      this.m_nsuOrigem = value.PadLeft(6, '0');
    }
  }

  public string nsu
  {
    get
    {
      return this.m_nsu;
    }
    set
    {
      this.setBit(37);
      this.m_nsu = value.PadLeft(6, '0');
    }
  }

  public ISO8583()
  {
    this.limpa();
  }

  public ISO8583(string reg)
  {
    this.limpa();
    this.registro = reg;
  }

  private void setBit(int bitnum)
  {
    if (bitnum > 128)
      return;
    if (bitnum < 65)
      this.mBits1.Set(bitnum, true);
    else
      this.mBits2.Set(bitnum - 64, true);
  }

  private void unsetBit(int bitnum)
  {
    if (bitnum > 128)
      return;
    if (bitnum < 65)
      this.mBits1.Set(bitnum, false);
    else
      this.mBits2.Set(bitnum - 64, false);
  }

  private void limpaBit(int bitnum)
  {
    if (bitnum > 128)
      return;
    if (bitnum < 65)
      this.mBits1.Set(bitnum, false);
    else
      this.mBits2.Set(bitnum - 64, false);
  }

  private bool getBit(int bitnum)
  {
    if (bitnum > 128)
      return false;
    if (bitnum < 65)
      return this.mBits1.Get(bitnum);
    return this.mBits2.Get(bitnum - 64);
  }

  private string monta_registro()
  {
    string str = "" + this.m_codigo + this.mapaBit1;
    for (int bitnum = 0; bitnum < 128; ++bitnum)
    {
      if (this.getBit(bitnum))
      {
        switch (bitnum)
        {
          case 120:
            str += this.m_bit120;
            break;
          case 125:
            int length1 = this.m_bit125.Length;
            str = str + length1.ToString("000") + this.m_bit125;
            break;
          case (int) sbyte.MaxValue:
            int length2 = this.m_bit127.Length;
            str = str + length2.ToString("000") + this.m_bit127;
            break;
          case 62:
            str += this.m_bit62;
            break;
          case 63:
            str += this.m_bit63;
            break;
          case 64:
            str += this.m_bit64;
            break;
          case 90:
            str += this.m_bit90;
            break;
          case 35:
            int length3 = this.m_trilha2.Length;
            str = str + length3.ToString("00") + this.m_trilha2;
            break;
          case 37:
            str += this.m_nsu;
            break;
          case 39:
            str += this.m_codResposta;
            break;
          case 41:
            str += this.m_terminal;
            break;
          case 42:
            str += this.m_codLoja;
            break;
          case 49:
            str += this.m_bit49;
            break;
          case 52:
            str += this.m_senha;
            break;
          case 53:
            str += this.m_security;
            break;
          case 1:
            str += this.mapaBit2;
            break;
          case 2:
            str += this.m_PAN;
            break;
          case 3:
            str += this.m_codProcessamento;
            break;
          case 4:
            str += this.m_valor;
            break;
          case 7:
            str += this.m_datetime;
            break;
          case 11:
            str += this.m_nsuOrigem;
            break;
          case 12:
            str += this.m_time;
            break;
          case 13:
            str += this.m_date;
            break;
          case 14:
            str += this.m_dateExp;
            break;
          case 22:
            str += this.m_bit22;
            break;
        }
      }
    }
    return str;
  }

  private void desmonta_registro(string regISO)
  {
    int num1 = 1;
    int length1 = regISO.Length;
    this.m_codigo = regISO.Substring(0, 4);
    this.m_mapaBit1 = regISO.Substring(4, 16);
    int num2 = 4;
    string str1 = "";
    string str2 = "";
    int startIndex1 = 0;
    while (startIndex1 < 16)
    {
      BitArray bitArray = new BitArray(BitConverter.GetBytes(int.Parse(this.m_mapaBit1.Substring(startIndex1, 2), NumberStyles.HexNumber)));
      for (int index = 0; index < 8; ++index)
      {
        str1 = !bitArray.Get(index) ? "0" + str1 : "1" + str1;
        ++num1;
      }
      str2 += str1;
      str1 = "";
      startIndex1 += 2;
    }
    int startIndex2 = num2 + 16;
    if (str2.Substring(0, 1) == "1")
    {
      this.m_mapaBit2 = regISO.Substring(20, 16);
      int startIndex3 = 0;
      while (startIndex3 < 16)
      {
        BitArray bitArray = new BitArray(BitConverter.GetBytes(int.Parse(this.m_mapaBit2.Substring(startIndex3, 2), NumberStyles.HexNumber)));
        for (int index = 0; index < 8; ++index)
        {
          str1 = !bitArray.Get(index) ? "0" + str1 : "1" + str1;
          ++num1;
        }
        str2 += str1;
        str1 = "";
        startIndex3 += 2;
      }
      startIndex2 += 16;
    }
    for (int startIndex3 = 0; startIndex3 < 128; ++startIndex3)
    {
      if (str2.Substring(startIndex3, 1) == "1")
        this.setBit(startIndex3 + 1);
    }
    for (int bitnum = 0; bitnum < 128; ++bitnum)
    {
      if (this.getBit(bitnum))
      {
        switch (bitnum)
        {
          case 120:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit120 = regISO.Substring(startIndex2, 3);
            startIndex2 += 3;
            int length2 = int.Parse(this.m_bit120);
            if (startIndex2 + length2 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit120 = this.m_bit120 + regISO.Substring(startIndex2, length2);
            startIndex2 += length2;
            break;
          case 125:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            string s1 = regISO.Substring(startIndex2, 3);
            startIndex2 += 3;
            int length3 = int.Parse(s1);
            if (startIndex2 + length3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit125 = regISO.Substring(startIndex2, length3);
            startIndex2 += length3;
            break;
          case (int) sbyte.MaxValue:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            string s2 = regISO.Substring(startIndex2, 3);
            startIndex2 += 3;
            int length4 = int.Parse(s2);
            if (startIndex2 + length4 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit127 = regISO.Substring(startIndex2, length4);
            startIndex2 += length4;
            break;
          case 62:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit62 = regISO.Substring(startIndex2, 3);
            startIndex2 += 3;
            int length5 = int.Parse(this.m_bit62);
            if (startIndex2 + length5 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit62 = this.m_bit62 + regISO.Substring(startIndex2, length5);
            startIndex2 += length5;
            break;
          case 63:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit63 = regISO.Substring(startIndex2, 3);
            startIndex2 += 3;
            int length6 = int.Parse(this.m_bit62);
            if (startIndex2 + length6 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit63 = this.m_bit63 + regISO.Substring(startIndex2, length6);
            startIndex2 += length6;
            break;
          case 64:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit64 = regISO.Substring(startIndex2, 3);
            int startIndex3 = startIndex2 + 3;
            int length7 = int.Parse(this.m_bit64);
            if (startIndex3 + length7 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
            }
            this.m_bit64 = this.m_bit64 + regISO.Substring(startIndex3, length7);
            startIndex2 = startIndex3 + length7;
            break;
          case 90:
            if (startIndex2 + 42 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit90 = regISO.Substring(startIndex2, 42);
            startIndex2 += 42;
            break;
          case 35:
            if (startIndex2 + 2 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            string s3 = regISO.Substring(startIndex2, 2);
            startIndex2 += 2;
            int length8 = int.Parse(s3);
            if (startIndex2 + length8 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_trilha2 = regISO.Substring(startIndex2, length8);
            startIndex2 += length8;
            break;
          case 37:
            if (startIndex2 + 6 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_nsu = regISO.Substring(startIndex2, 6);
            startIndex2 += 6;
            break;
          case 39:
            if (startIndex2 + 2 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_codResposta = regISO.Substring(startIndex2, 2);
            startIndex2 += 2;
            break;
          case 41:
            if (startIndex2 + 8 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_terminal = regISO.Substring(startIndex2, 8);
            startIndex2 += 8;
            break;
          case 42:
            if (startIndex2 + 15 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_codLoja = regISO.Substring(startIndex2, 15);
            startIndex2 += 15;
            break;
          case 49:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit49 = regISO.Substring(startIndex2, 3);
            startIndex2 += 3;
            break;
          case 52:
            if (startIndex2 + 16 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_senha = regISO.Substring(startIndex2, 16);
            startIndex2 += 16;
            break;
          case 53:
            if (startIndex2 + 6 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_security = regISO.Substring(startIndex2, 6);
            startIndex2 += 6;
            break;
          case 2:
            if (startIndex2 + 19 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_PAN = regISO.Substring(startIndex2, 19);
            startIndex2 += 19;
            break;
          case 3:
            if (startIndex2 + 6 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_codProcessamento = regISO.Substring(startIndex2, 6);
            startIndex2 += 6;
            break;
          case 4:
            if (startIndex2 + 12 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_valor = regISO.Substring(startIndex2, 12);
            startIndex2 += 12;
            break;
          case 7:
            if (startIndex2 + 10 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_datetime = regISO.Substring(startIndex2, 10);
            startIndex2 += 10;
            break;
          case 11:
            if (startIndex2 + 6 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_nsuOrigem = regISO.Substring(startIndex2, 6);
            startIndex2 += 6;
            break;
          case 12:
            if (startIndex2 + 6 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_time = regISO.Substring(startIndex2, 6);
            startIndex2 += 6;
            break;
          case 13:
            if (startIndex2 + 4 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_date = regISO.Substring(startIndex2, 4);
            startIndex2 += 4;
            break;
          case 14:
            if (startIndex2 + 4 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_dateExp = regISO.Substring(startIndex2, 4);
            startIndex2 += 4;
            break;
          case 22:
            if (startIndex2 + 3 > length1)
            {
              Util.LOGERRO("registro com erro no bit ( " + bitnum.ToString() + ")");
              this.m_erro = true;
              continue;
            }
            this.m_bit22 = regISO.Substring(startIndex2, 3);
            startIndex2 += 3;
            break;
        }
      }
    }
  }

  public void limpa()
  {
    for (int index = 0; index < 128; ++index)
      this.unsetBit(index + 1);
    this.m_codigo = "0000";
    this.m_codProcessamento = "000000";
    this.m_PAN = "0000000000000000000";
    this.m_datetime = DateTime.Now.ToString("MMddHHmmss");
    DateTime now = DateTime.Now;
    this.m_date = now.ToString("MMdd");
    now = DateTime.Now;
    this.m_time = now.ToString("HHmmss");
    this.m_codResposta = "00";
    this.m_senha = "                ";
    this.m_security = "";
    this.m_dateExp = "";
    this.m_terminal = "00000000";
    this.m_bit22 = "";
    this.m_bit62 = "";
    this.m_bit64 = "";
    this.m_codLoja = "";
    this.setBit(1);
    this.setBit(7);
    this.setBit(12);
    this.setBit(13);
  }
}
