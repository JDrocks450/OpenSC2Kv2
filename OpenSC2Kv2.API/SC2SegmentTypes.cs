namespace OpenSC2Kv2.API
{
    public enum SC2BuildingTypes
    {
        /// <summary>
        /// OBJECTS
        /// </summary>
        CLEAR = 0x00,
        RUBBLE = 0x01,
        RUBBLE2 = 0x02,
        RUBBLE3 = 0x03,
        RUBBLE4 = 0x04,
        RADIOACTIVE_WASTE = 0x05,
        TREE_DENSITY_1 = 0x06,
        TREE_DENSITY_2 = 0x07,
        TREE_DENSITY_3 = 0x08,
        TREE_DENSITY_4 = 0x09,
        TREE_DENSITY_5 = 0x0A,
        TREE_DENSITY_6 = 0x0B,
        TREE_DENSITY_7 = 0x0C,
        PARK_SM = 0x0D,
        POWER_LINE = 0x0E,
        ROAD = 0x1D,
        RAIL = 0x2C,
        RAIL_SPECIAL_VERTICAL_SLOPE_N = 0x3B,
        RAIL_SPECIAL_VERTICAL_SLOPE_E = 0x3C,
        RAIL_SPECIAL_VERTICAL_SLOPE_S = 0x3D,
        RAIL_SPECIAL_VERTICAL_SLOPE_W = 0x3E,
        TUNNEL_N = 0x3F,
        TUNNEL_E = 0x40,
        TUNNEL_S = 0x41,
        TUNNEL_W = 0x42,
        ROAD_POWERED_HORIZONTAL = 0x43,
        ROAD_POWERED_VERTICAL = 0x44,
        ROAD_RAILED_HORIZONTAL = 0x45,
        ROAD_RAILED_VERTICAL = 0x46,
        RAIL_POWERED_HORIZONTAL = 0x47,
        RAIL_POWERED_VERTICAL = 0x48,
        HIGHWAY_HORIZONTAL = 0x49,
        HIGHWAY_VERTICAL = 0x4A,
        HIGHWAY_ROAD_HORIZONTAL = 0x4B,
        HIGHWAY_ROAD_VERTICAL = 0x4C,
        HIGHWAY_RAIL_HORIZONTAL = 0x4D,
        HIGHWAY_RAIL_VERTICAL = 0x4E,
        HIGHWAY_POWERED_HORIZONTAL = 0x4F,
        HIGHWAY_POWERED_VERTICAL = 0x50,
        SUSPENSION_BRIDGE = 0x51,
        SUSPENSION_BRIDGE2 = 0x52,
        SUSPENSION_BRIDGE3 = 0x53,
        SUSPENSION_BRIDGE4 = 0x54,
        SUSPENSION_BRIDGE5 = 0x55,
        BRIDGE = 0x56,
        BRIDGE2 = 0x57,
        BRIDGE3 = 0x58,
        BRIDGE4 = 0x59,
        RAIL_BRIDGE = 0x5A,
        RAIL_BRIDGE2 = 0x5B,
        ELEVATED_PWR_LINE = 0x5C,
        ONRAMP1 = 0x5D,
        ONRAMP2 = 0x5E,
        ONRAMP3 = 0x5F,
        ONRAMP4 = 0x60,
        HIGHWAY = 0x61,
        REINFORCED_HIWAY = 0x6A,
        REINFORCED_HIWAY2 = 0x6B,
        SUB_RAIL_S = 0x6C,
        SUB_RAIL_W = 0x6D,
        SUB_RAIL_N = 0x6E,
        SUB_RAIL_E = 0x6F,

        //BUILDINGS
        RESI_LOW_SMALL = 0x70,
        RESI_LOW_SMALL2 = 0x71,
        RESI_LOW_SMALL3 = 0x72,
        RESI_LOW_SMALL4 = 0x73,
        RESI_MED_SMALL = 0x74,
        RESI_MED_SMALL2 = 0x75,
        RESI_MED_SMALL3 = 0x76,
        RESI_MED_SMALL4 = 0x77,

        //2x2
        APART_CHEAP = 0x8C,
        APARTMENTS = 0x8D,
        APARTMENTS2 = 0x8E,
        APARTMENT_NICE = 0x8F,
        APARTMENT_NICE2 = 0x90,
        CONDO = 0x91,
        CONDO2 = 0x92,
        CONDO3 = 0x93,

        //Commercial, 2x2:
        SHOPCTR = 94,
        GROC = 95,

    /*94: Shopping center
    95: Grocery store
    96: Office building
    97: Resort hotel
    98: Office building
    99: Office / Retail
    9A-9D: Office building
  Industrial, 2x2:
    9E: Warehouse
    9F: Chemical processing
    A0-A5: Factory
  Miscellaneous, 2x2:
    A6-A9: Construction
    AA-AD: Abandoned building
  Residential, 3x3:
    AE-AF: Large apartment building
    B0-B1: Condominium
  Commercial, 3x3:
    B2: Office park
    B3: Office tower
    B4: Mini-mall
    B5: Theater square
    B6: Drive-in theater
    B7-B8: Office tower
    B9: Parking lot
    BA: Historic office building
    BB: Corporate headquarters
  Industrial, 3x3:
    BC: Chemical processing
    BD: Large factory
    BE: Industrial thingamajig
    BF: Factory
    C0: Large warehouse
    C1: Warehouse
  Miscellaneous, 3x3:
    C2-C3: Construction
    C4-C5: Abandoned building
  Power plants:
    C6-C7: Hydroelectric power (1x1)
    C8: Wind power (1x1)
    C9: Natural gas power plant (4x4)
    CA: Oil power plant (4x4)
    CB: Nuclear power plant (4x4)
    CC: Solar power plant (4x4)
    CD: Microwave power receiver (4x4)
    CE: Fusion power plant (4x4)
    CF: Coal power plant (4x4)
  City services:
    D0: City hall
    D1: Hospital
    D2: Police station
    D3: Fire station
    D4: Museum
    D5: Park (big)
    D6: School
    D7: Stadium
    D8: Prison
    D9: College
    DA: Zoo*/
    }

    /// <summary>
    /// Some tiles are connectables: Roads, Powerlines, etc.
    /// <para>These directions, when added to their <see cref="SC2BuildingTypes"/> value
    /// will result in the selected modification.</para>
    /// </summary>
    public enum SC2ConnectableOffsets
    {
        /// <summary>
        /// Left-right
        /// </summary>
        HORIZONTAL = 0,
        /// Top-bottom
        VERTICAL = 1,   
        /// Top-bottom; slopes upwards towards top
        VERTICAL_SLOPE_N = 2,
        /// <summary>
        /// Left-right; slopes upwards towards right 
        /// </summary>
        HORIZONTAL_SLOPE_E = 3,
        /// <summary>
        /// Top-bottom; slopes upwards towards bottom
        /// </summary>
        VERTICAL_SLOPE_S = 4,
        /// <summary>
        /// Left-right; slopes upwards towards left
        /// </summary>
        HORIZONTAL_SLOPE_W = 5,
        /// <summary>
        /// From bottom side to right side
        /// </summary>
        TURN_S_E = 6,
        /// <summary>
        /// Bottom to left
        /// </summary>
        TURN_S_W = 7,
        /// <summary>
        /// Left to top
        /// </summary>
        TURN_W_N,
        /// <summary>
        /// Top to right
        /// </summary>
        TURN_N_E,
        /// <summary>
        /// T junction between top, right and bottom
        /// </summary>
        T_N_E, S,
        /// <summary>
        /// T between left, bottom and right
        /// </summary>
        T_W_S_E,
        /// <summary>
        /// T between top, left and bottom
        /// </summary>
        T_N_W_S,
        /// <summary>
        /// T between top, left and right
        /// </summary>
        T_N_W_E,
        /// <summary>
        /// Intersection connecting top, left, bottom, and right
        /// </summary>
        ALL_DIRECTIONS
    }

    /// <summary>
    /// More information: http://djm.cc/simcity-2000-info.txt
    /// </summary>
    public enum SC2SegmentTypes
    {
        /// <summary>
        /// Any data point that is not significant enough to have it's own segment type.
        /// <para>1200 length, Integer Type values.</para>
        /// </summary>
        MISC,
        /// <summary>
        /// Altitude map.  Uncompressed.  Contains two bytes for each square.
        /// (For our purposes, we will define `left', `right', `top', and `bottom'
        /// by saying that squares are scanned by rows from top to bottom, and
        /// from right to left within each row.)
        /// Taking each two bytes as a 16-bit integer, MSB first, bits 4-0
        /// give the altitude of the square, from 50 to 3150 feet.  Bit 7 seems
        /// to be set if the square is covered with water.  I do not know what
        /// bits 15-8 and 6-5 do.
        /// </summary>
        ALTM,
        /// <summary>
        /// 
        /// </summary>
        XTER,
        /// <summary>
        /// Describes the building type on the tile.
        /// </summary>
        XBLD,
        XZON,
        XUND,
        XTXT,
        XLAB,
        XMIC,
        XTHG,
        XBIT,
        XTRF,
        XPLT,
        XVAL,
        XCRM,
        XPLC,
        XFIR,
        XPOP,
        XROG,
        XGRP,
        /// <summary>
        /// City name.  Uncompressed.  Seems to be optional.  When it is present,
        /// it consists of a length byte from 0 to 31, followed by that many
        /// bytes of city name.  It is padded out to 32 bytes with zeroes.
        /// </summary>
        CNAM,
    }
}
