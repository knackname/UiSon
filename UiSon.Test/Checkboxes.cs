// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    public class Checkboxes
    {
        [UiSonCheckboxUi]
        public string String_CheckboxUi_Inited_bad = "Init";
        [UiSonCheckboxUi]
        public string String_CheckboxUi_Inited_good = "true";
        [UiSonCheckboxUi]
        public string String_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public sbyte Sbyte_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public sbyte Sbyte_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public sbyte? NUllableSbyte_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public byte Byte_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public byte Byte_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public byte? NullableByte_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public short Short_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public short Short_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public short? NullableShort_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public ushort Ushort_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public ushort Ushort_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public ushort? NullableUshort_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public int Int_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public int Int_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public int? NullableInt_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public uint Uint_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public uint Uint_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public uint? NullableUint_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public long Long_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public long Long_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public long? NullableLong_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public ulong Ulong_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public ulong Ulong_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public ulong? NullableUlong_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public nint Nint_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public nint Nint_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public nint? NullableNint_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public nuint Nuint_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public nuint Nuint_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public nuint? NullableNuint_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public float Float_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public float Float_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public float? NullableFloat_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public double Double_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public double Double_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public double? NullableDouble_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public decimal Decimal_CheckboxUi_Inited = 100;
        [UiSonCheckboxUi]
        public decimal Decimal_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public decimal? NullableDecimal_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public bool Bool_CheckboxUi_Inited = true;
        [UiSonCheckboxUi]
        public bool Bool_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public bool? NullableBool_CheckboxUi_Uninited;

        [UiSonCheckboxUi]
        public char Char_CheckboxUi_Inited = 'K';
        [UiSonCheckboxUi]
        public char Char_CheckboxUi_Uninited;
        [UiSonCheckboxUi]
        public char? NullableChar_CheckboxUi_Uninited;
    }
}
