﻿using System;
using System.Collections.Generic;

namespace Kafka.DotNet.ksqlDB.KSql.Query.Functions
{
  public static class KSqlFunctionsExtensions
  {
    private static string ServerSideOperationErrorMessage = "Operator is not intended for client side operations";
    
    public static object Dynamic(this KSqlFunctions kSqlFunctions, string functionCall)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #region Numeric

    #region Abs

    /// <summary>
    /// Returns the absolute value of its argument. If the argument is not negative, the argument is returned. If
    /// the argument is negative, the negation of the argument is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Absolute value of its argument</returns>
    public static int Abs(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns the absolute value of its argument. If the argument is not negative, the argument is returned. If
    /// the argument is negative, the negation of the argument is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Absolute value of its argument</returns>
    public static long Abs(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the absolute value of its argument. If the argument is not negative, the argument is returned. If
    /// the argument is negative, the negation of the argument is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Absolute value of its argument</returns>
    public static float Abs(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the absolute value of its argument. If the argument is not negative, the argument is returned. If
    /// the argument is negative, the negation of the argument is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Absolute value of its argument</returns>
    public static double Abs(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the absolute value of its argument. If the argument is not negative, the argument is returned. If
    /// the argument is negative, the negation of the argument is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Absolute value of its argument</returns>
    public static decimal Abs(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion    
    
    #region Ceil
    
    /// <summary>
    /// Returns the smallest integer greater than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The ceiling of a value.</returns>
    public static int Ceil(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the smallest integer greater than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The ceiling of a value.</returns>
    public static long Ceil(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the smallest integer greater than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The ceiling of a value.</returns>
    public static float Ceil(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the smallest integer greater than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The ceiling of a value.</returns>
    public static double Ceil(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the smallest integer greater than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The ceiling of a value.</returns>
    public static decimal Ceil(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Entries

    //Variation   : ENTRIES(map MAP<VARCHAR, VARCHAR>, sorted BOOLEAN)
    //Returns     : ARRAY<STRUCT<K VARCHAR, V VARCHAR>>
    //map         : The map to create entries from
    //sorted      : If true then the resulting entries are sorted by key
    
    /// <summary>
    /// Constructs an array of Entry structs from the entries in a map. Each struct has a field named K containing the key, which is a string, and a field named V, which holds the value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="map">The map to create entries from.</param>
    /// <param name="sorted">If true then the resulting entries are sorted by key.</param>
    /// <returns></returns>
    public static Entry<string>[] Entries(this KSqlFunctions kSqlFunctions, IDictionary<string, string> map, bool sorted)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    //Variation   : ENTRIES(map MAP<VARCHAR, BIGINT>, sorted BOOLEAN)
    //Returns     : ARRAY<STRUCT<K VARCHAR, V BIGINT>>
    //map         : The map to create entries from
    //sorted      : If true then the resulting entries are sorted by key
    
    /// <summary>
    /// Constructs an array of Entry structs from the entries in a map. Each struct has a field named K containing the key, which is a string, and a field named V, which holds the value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="map">The map to create entries from.</param>
    /// <param name="sorted">If true then the resulting entries are sorted by key.</param>
    /// <returns></returns>
    public static Entry<long>[] Entries(this KSqlFunctions kSqlFunctions, IDictionary<string, long> map, bool sorted)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    //Variation   : ENTRIES(map MAP<VARCHAR, INT>, sorted BOOLEAN)
    //Returns     : ARRAY<STRUCT<K VARCHAR, V INT>>
    //map         : The map to create entries from
    //sorted      : If true then the resulting entries are sorted by key
        
    /// <summary>
    /// Constructs an array of Entry structs from the entries in a map. Each struct has a field named K containing the key, which is a string, and a field named V, which holds the value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="map">The map to create entries from.</param>
    /// <param name="sorted">If true then the resulting entries are sorted by key.</param>
    /// <returns></returns>
    public static Entry<int>[] Entries(this KSqlFunctions kSqlFunctions, IDictionary<string, int> map, bool sorted)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    //Variation   : ENTRIES(map MAP<VARCHAR, DOUBLE>, sorted BOOLEAN)
    //Returns     : ARRAY<STRUCT<K VARCHAR, V DOUBLE>>
    //map         : The map to create entries from
    //sorted      : If true then the resulting entries are sorted by key
        
    /// <summary>
    /// Constructs an array of Entry structs from the entries in a map. Each struct has a field named K containing the key, which is a string, and a field named V, which holds the value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="map">The map to create entries from.</param>
    /// <param name="sorted">If true then the resulting entries are sorted by key.</param>
    /// <returns></returns>
    public static Entry<double>[] Entries(this KSqlFunctions kSqlFunctions, IDictionary<string, double> map, bool sorted)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    //Variation   : ENTRIES(map MAP<VARCHAR, BOOLEAN>, sorted BOOLEAN)
    //Returns     : ARRAY<STRUCT<K VARCHAR, V BOOLEAN>>
    //map         : The map to create entries from
    //sorted      : If true then the resulting entries are sorted by key
    
    /// <summary>
    /// Constructs an array of Entry structs from the entries in a map. Each struct has a field named K containing the key, which is a string, and a field named V, which holds the value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="map">The map to create entries from.</param>
    /// <param name="sorted">If true then the resulting entries are sorted by key.</param>
    /// <returns></returns>
    public static Entry<bool>[] Entries(this KSqlFunctions kSqlFunctions, IDictionary<string, bool> map, bool sorted)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Exp

    /// <summary>
    /// The exponential of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Euler's number e raised to the power of an DOUBLE value.</returns>
    public static double Exp(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// The exponential of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Euler's number e raised to the power of an INT value.</returns>
    public static int Exp(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// The exponential of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Euler's number e raised to the power of an BIGINT value.</returns>
    public static long Exp(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Floor
    
    /// <summary>
    /// Returns the largest integer less than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The floor of a value.</returns>
    public static int Floor(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the largest integer less than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The floor of a value.</returns>
    public static long Floor(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the largest integer less than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The floor of a value.</returns>
    public static float Floor(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the largest integer less than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The floor of a value.</returns>
    public static double Floor(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Returns the largest integer less than or equal to the specified numeric expression.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>The floor of a value.</returns>
    public static decimal Floor(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region GenerateSeries

    /// <summary>
    /// Constructs an array of values between start and end (inclusive).
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="start">The beginning of the series</param>
    /// <param name="end">Marks the end of the series (inclusive)</param>
    /// <returns>Constructed array of values between start and end (inclusive)</returns>
    public static int[] GenerateSeries(this KSqlFunctions kSqlFunctions, int start, int end)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Constructs an array of values between start and end (inclusive).
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="start">The beginning of the series</param>
    /// <param name="end">Marks the end of the series (inclusive)</param>
    /// <returns>Constructed array of values between start and end (inclusive)</returns>
    public static long[] GenerateSeries(this KSqlFunctions kSqlFunctions, long start, long end)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Constructs an array of values between start and end (inclusive).
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="start">The beginning of the series</param>
    /// <param name="step">Difference between each value in the series</param>
    /// <returns>Constructed array of values between start and end (inclusive)</returns>
    public static int[] GenerateSeries(this KSqlFunctions kSqlFunctions, int start, int end, int step)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Constructs an array of values between start and end (inclusive).
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="start">The beginning of the series</param>
    /// <param name="step">Difference between each value in the series</param>
    /// <returns>Constructed array of values between start and end (inclusive)</returns>
    public static long[] GenerateSeries(this KSqlFunctions kSqlFunctions, long start, long end, int step)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region GeoDistance

    /// <summary>
    /// The 2 input points should be specified as (lat, lon) pairs, measured in decimal degrees.
    /// Unit for the output measurement is KM.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="lat1">The latitude of the first point in decimal degrees.</param>
    /// <param name="lon1">The longitude of the first point in decimal degrees.</param>
    /// <param name="lat2">The latitude of the second point in decimal degrees.</param>
    /// <param name="lon2">The longitude of the second point in decimal degrees.</param>
    /// <returns>Computed distance between two points on the surface of the earth, according to the Haversine formula
    /// for "great circle distance".</returns>
    public static double GeoDistance(this KSqlFunctions kSqlFunctions, double lat1, double lon1, double lat2, double lon2)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// The 2 input points should be specified as (lat, lon) pairs, measured in decimal degrees. An optional
    /// fifth parameter allows to specify either "MI" (miles) or "KM" (kilometers) as the desired unit for the
    /// output measurement.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="lat1">The latitude of the first point in decimal degrees.</param>
    /// <param name="lon1">The longitude of the first point in decimal degrees.</param>
    /// <param name="lat2">The latitude of the second point in decimal degrees.</param>
    /// <param name="lon2">The longitude of the second point in decimal degrees.</param>
    /// <param name="units"></param>
    /// <returns>Computed distance between two points on the surface of the earth, according to the Haversine formula
    /// for "great circle distance".</returns>
    public static double GeoDistance(this KSqlFunctions kSqlFunctions, double lat1, double lon1, double lat2, double lon2, string units)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region LN

    /// <summary>
    /// The natural logarithm of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="value">The value get the natural logarithm of.</param>
    /// <returns>Returns the natural logarithm (base e) of a INT value.</returns>
    public static double Ln(this KSqlFunctions kSqlFunctions, int value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }


    /// <summary>
    /// The natural logarithm of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="value">The value get the natural logarithm of.</param>
    /// <returns>Returns the natural logarithm (base e) of a BIGINT value.</returns>
    public static double Ln(this KSqlFunctions kSqlFunctions, long value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// The natural logarithm of a value..
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="value">The value get the natural logarithm of.</param>
    /// <returns>Returns the natural logarithm (base e) of a DOUBLE value.</returns>
    public static double Ln(this KSqlFunctions kSqlFunctions, double value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Random

    /// <summary>
    /// Returns a random number greater than or equal to 0.0 and less than 1.0.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <returns>A random DOUBLE value between 0.0 and 1.0.</returns>
    public static double Random(this KSqlFunctions kSqlFunctions)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Round
    
    /// <summary>
    /// Round a value to the number of decimal places as specified by scale to the right of the decimal point. If
    /// scale is negative then value is rounded to the right of the decimal point. Numbers equidistant to the
    /// nearest value are rounded up (in the positive direction). If the number of decimal places is not provided
    /// it defaults to zero.
    /// </summary>
    public static float Round(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Round a value to the number of decimal places as specified by scale to the right of the decimal point. If
    /// scale is negative then value is rounded to the right of the decimal point. Numbers equidistant to the
    /// nearest value are rounded up (in the positive direction). If the number of decimal places is not provided
    /// it defaults to zero.
    /// </summary>
    public static double Round(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Round a value to the number of decimal places as specified by scale to the right of the decimal point. If
    /// scale is negative then value is rounded to the right of the decimal point. Numbers equidistant to the
    /// nearest value are rounded up (in the positive direction). If the number of decimal places is not provided
    /// it defaults to zero.
    /// </summary>
    public static decimal Round(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Round a value to the number of decimal places as specified by scale to the right of the decimal point. If
    /// scale is negative then value is rounded to the right of the decimal point. Numbers equidistant to the
    /// nearest value are rounded up (in the positive direction). If the number of decimal places is not provided
    /// it defaults to zero.
    /// </summary>
    public static float Round(this KSqlFunctions kSqlFunctions, float input, int scale)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Round a value to the number of decimal places as specified by scale to the right of the decimal point. If
    /// scale is negative then value is rounded to the right of the decimal point. Numbers equidistant to the
    /// nearest value are rounded up (in the positive direction). If the number of decimal places is not provided
    /// it defaults to zero.
    /// </summary>
    public static double Round(this KSqlFunctions kSqlFunctions, double input, int scale)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Round a value to the number of decimal places as specified by scale to the right of the decimal point. If
    /// scale is negative then value is rounded to the right of the decimal point. Numbers equidistant to the
    /// nearest value are rounded up (in the positive direction). If the number of decimal places is not provided
    /// it defaults to zero.
    /// </summary>
    public static decimal Round(this KSqlFunctions kSqlFunctions, decimal input, int scale)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Sign

    /// <summary>
    /// The sign of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Returns the sign of an INT value, denoted by 1, 0 or -1.</returns>
    public static int Sign(this KSqlFunctions kSqlFunctions, short input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// The sign of a value.
    /// </summary>
    public static int Sign(this KSqlFunctions kSqlFunctions, int input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// The sign of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Returns the sign of an BIGINT value, denoted by 1, 0 or -1.</returns>
    public static int Sign(this KSqlFunctions kSqlFunctions, long input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// The sign of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Returns the sign of an BIGINT value, denoted by 1, 0 or -1. null argument is null.</returns>
    public static int Sign(this KSqlFunctions kSqlFunctions, long? input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// The sign of a value.
    /// </summary>
    public static int Sign(this KSqlFunctions kSqlFunctions, float input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// The sign of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Returns the sign of an DOUBLE value, denoted by 1, 0 or -1.</returns>
    public static int Sign(this KSqlFunctions kSqlFunctions, double input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// The sign of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input"></param>
    /// <returns>Returns the sign of an DOUBLE value, denoted by 1, 0 or -1. null argument is null.</returns>
    public static int Sign(this KSqlFunctions kSqlFunctions, double? input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// The sign of a value.
    /// </summary>
    [Obsolete]
    public static int Sign(this KSqlFunctions kSqlFunctions, decimal input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Sqrt
    
    /// <summary>
    /// The square root of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="value">The value to get the square root of</param>
    /// <returns>Returns the correctly rounded positive square root of a INT value</returns>
    public static double Sqrt(this KSqlFunctions kSqlFunctions, int value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// The square root of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="value">The value to get the square root of</param>
    /// <returns>Returns the correctly rounded positive square root of a DOUBLE value</returns>
    public static double Sqrt(this KSqlFunctions kSqlFunctions, double value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// The square root of a value.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="value">The value to get the square root of</param>
    /// <returns>Returns the correctly rounded positive square root of a BIGINT value</returns>
    public static double Sqrt(this KSqlFunctions kSqlFunctions, long value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #endregion

    #region Collections

    //TODO:

    #region ArrayContains

    /// <summary>
    /// Accepts any ARRAY type. The type of the second param must match the element type of the ARRAY
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Given an array, checks if a search value is contained in the array.</param>
    /// <param name="value">Value to find</param>
    /// <returns>Returns true if the array is non-null and contains the supplied value.</returns>
    public static bool ArrayContains(this KSqlFunctions kSqlFunctions, string[] array, string value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Accepts any ARRAY type. The type of the second param must match the element type of the ARRAY
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Given an array, checks if a search value is contained in the array.</param>
    /// <param name="value">Value to find</param>
    /// <returns>Returns true if the array is non-null and contains the supplied value.</returns>
    public static bool ArrayContains(this KSqlFunctions kSqlFunctions, int[] array, int value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Accepts any ARRAY type. The type of the second param must match the element type of the ARRAY
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Given an array, checks if a search value is contained in the array.</param>
    /// <param name="value">Value to find</param>
    /// <returns>Returns true if the array is non-null and contains the supplied value.</returns>
    public static bool ArrayContains(this KSqlFunctions kSqlFunctions, double[] array, double value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Accepts any ARRAY type. The type of the second param must match the element type of the ARRAY
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Given an array, checks if a search value is contained in the array.</param>
    /// <param name="value">Value to find</param>
    /// <returns>Returns true if the array is non-null and contains the supplied value.</returns>
    public static bool ArrayContains(this KSqlFunctions kSqlFunctions, long[] array, long value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Accepts any ARRAY type. The type of the second param must match the element type of the ARRAY
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Given an array, checks if a search value is contained in the array.</param>
    /// <param name="value">Value to find</param>
    /// <returns>Returns true if the array is non-null and contains the supplied value.</returns>
    public static bool ArrayContains(this KSqlFunctions kSqlFunctions, decimal[] array, decimal value)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region ArrayDistinct

    /// <summary>
    /// Returns an array of all the distinct values, including NULL if present, from the input array. The output
    /// array elements will be in order of their first occurrence in the input. Returns NULL if the input array is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Array of values to distinct</param>
    /// <returns></returns>
    public static string[] ArrayDistinct(this KSqlFunctions kSqlFunctions, string[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct values, including NULL if present, from the input array. The output
    /// array elements will be in order of their first occurrence in the input. Returns NULL if the input array is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Array of values to distinct</param>
    /// <returns></returns>
    public static int[] ArrayDistinct(this KSqlFunctions kSqlFunctions, int[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct values, including NULL if present, from the input array. The output
    /// array elements will be in order of their first occurrence in the input. Returns NULL if the input array is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Array of values to distinct</param>
    /// <returns></returns>
    public static long[] ArrayDistinct(this KSqlFunctions kSqlFunctions, long[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct values, including NULL if present, from the input array. The output
    /// array elements will be in order of their first occurrence in the input. Returns NULL if the input array is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Array of values to distinct</param>
    /// <returns></returns>
    public static double[] ArrayDistinct(this KSqlFunctions kSqlFunctions, double[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct values, including NULL if present, from the input array. The output
    /// array elements will be in order of their first occurrence in the input. Returns NULL if the input array is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">Array of values to distinct</param>
    /// <returns></returns>
    public static decimal[] ArrayDistinct(this KSqlFunctions kSqlFunctions, decimal[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region ArrayExcept

    /// <summary>
    /// Returns an array of all the elements in an array except for those also present in a second array. The
    /// order of entries in the first array is preserved although any duplicates are removed. Returns NULL if
    /// either input is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">Array of values</param>
    /// <param name="right">Array of exceptions</param>
    /// <returns></returns>
    public static string[] ArrayExcept(this KSqlFunctions kSqlFunctions, string[] left, string[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the elements in an array except for those also present in a second array. The
    /// order of entries in the first array is preserved although any duplicates are removed. Returns NULL if
    /// either input is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">Array of values</param>
    /// <param name="right">Array of exceptions</param>
    /// <returns></returns>
    public static int[] ArrayExcept(this KSqlFunctions kSqlFunctions, int[] left, int[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the elements in an array except for those also present in a second array. The
    /// order of entries in the first array is preserved although any duplicates are removed. Returns NULL if
    /// either input is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">Array of values</param>
    /// <param name="right">Array of exceptions</param>
    /// <returns></returns>
    public static long[] ArrayExcept(this KSqlFunctions kSqlFunctions, long[] left, long[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the elements in an array except for those also present in a second array. The
    /// order of entries in the first array is preserved although any duplicates are removed. Returns NULL if
    /// either input is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">Array of values</param>
    /// <param name="right">Array of exceptions</param>
    /// <returns></returns>
    public static double[] ArrayExcept(this KSqlFunctions kSqlFunctions, double[] left, double[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the elements in an array except for those also present in a second array. The
    /// order of entries in the first array is preserved although any duplicates are removed. Returns NULL if
    /// either input is NULL.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">Array of values</param>
    /// <param name="right">Array of exceptions</param>
    /// <returns></returns>
    public static decimal[] ArrayExcept(this KSqlFunctions kSqlFunctions, decimal[] left, decimal[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region ArrayIntersect 
    
    /// <summary>
    /// Returns an array of all the distinct elements from the intersection of both input arrays, or NULL if
    /// either input array is NULL. The order of entries in the output is the same as in the first input array.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">First array of values</param>
    /// <param name="right">Second array of values</param>
    /// <returns></returns>
    public static string[] ArrayIntersect(this KSqlFunctions kSqlFunctions, string[] left, string[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct elements from the intersection of both input arrays, or NULL if
    /// either input array is NULL. The order of entries in the output is the same as in the first input array.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">First array of values</param>
    /// <param name="right">Second array of values</param>
    /// <returns></returns>
    public static int[] ArrayIntersect(this KSqlFunctions kSqlFunctions, int[] left, int[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct elements from the intersection of both input arrays, or NULL if
    /// either input array is NULL. The order of entries in the output is the same as in the first input array.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">First array of values</param>
    /// <param name="right">Second array of values</param>
    /// <returns></returns>
    public static long[] ArrayIntersect(this KSqlFunctions kSqlFunctions, long[] left, long[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct elements from the intersection of both input arrays, or NULL if
    /// either input array is NULL. The order of entries in the output is the same as in the first input array.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">First array of values</param>
    /// <param name="right">Second array of values</param>
    /// <returns></returns>
    public static double[] ArrayIntersect(this KSqlFunctions kSqlFunctions, double[] left, double[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns an array of all the distinct elements from the intersection of both input arrays, or NULL if
    /// either input array is NULL. The order of entries in the output is the same as in the first input array.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="left">First array of values</param>
    /// <param name="right">Second array of values</param>
    /// <returns></returns>
    public static decimal[] ArrayIntersect(this KSqlFunctions kSqlFunctions, decimal[] left, decimal[] right)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region ArrayJoin

    /// <summary>
    /// Creates a flat string representation of all the elements contained in the given array. The elements in the resulting string are separated by the chosen delimiter.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array to join using the specified delimiter</param>
    /// <param name="delimiter">The string to be used as element delimiter</param>
    /// <returns></returns>
    public static string ArrayJoin(this KSqlFunctions kSqlFunctions, string[] array, string delimiter)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Creates a flat string representation of all the elements contained in the given array. The elements in the resulting string are separated by the chosen delimiter.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array to join using the specified delimiter</param>
    /// <param name="delimiter">The string to be used as element delimiter</param>
    /// <returns></returns>
    public static string ArrayJoin(this KSqlFunctions kSqlFunctions, int[] array, string delimiter)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Creates a flat string representation of all the elements contained in the given array. The elements in the resulting string are separated by the chosen delimiter.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array to join using the specified delimiter</param>
    /// <param name="delimiter">The string to be used as element delimiter</param>
    /// <returns></returns>
    public static string ArrayJoin(this KSqlFunctions kSqlFunctions, long[] array, string delimiter)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Creates a flat string representation of all the elements contained in the given array. The elements in the resulting string are separated by the chosen delimiter.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array to join using the specified delimiter</param>
    /// <param name="delimiter">The string to be used as element delimiter</param>
    /// <returns></returns>
    public static string ArrayJoin(this KSqlFunctions kSqlFunctions, double[] array, string delimiter)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Creates a flat string representation of all the elements contained in the given array. The elements in the resulting string are separated by the chosen delimiter.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array to join using the specified delimiter</param>
    /// <param name="delimiter">The string to be used as element delimiter</param>
    /// <returns></returns>
    public static string ArrayJoin(this KSqlFunctions kSqlFunctions, decimal[] array, string delimiter)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region ArrayLength

    /// <summary>
    /// Given an array, return the number of elements in the array. If the array field is NULL, or contains only NULLs, then NULL is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array</param>
    /// <returns>Returns the maximum value from within a given array of primitive elements (not arrays of other arrays, or maps, or structs, or combinations thereof).</returns>
    public static int? ArrayLength(this KSqlFunctions kSqlFunctions, string[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Given an array, return the number of elements in the array. If the array field is NULL, or contains only NULLs, then NULL is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array</param>
    /// <returns>Returns the maximum value from within a given array of primitive elements (not arrays of other arrays, or maps, or structs, or combinations thereof).</returns>
    public static int? ArrayLength(this KSqlFunctions kSqlFunctions, int[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Given an array, return the number of elements in the array. If the array field is NULL, or contains only NULLs, then NULL is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array</param>
    /// <returns>Returns the maximum value from within a given array of primitive elements (not arrays of other arrays, or maps, or structs, or combinations thereof).</returns>
    public static int? ArrayLength(this KSqlFunctions kSqlFunctions, long[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Given an array, return the number of elements in the array. If the array field is NULL, or contains only NULLs, then NULL is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array</param>
    /// <returns>Returns the maximum value from within a given array of primitive elements (not arrays of other arrays, or maps, or structs, or combinations thereof).</returns>
    public static int? ArrayLength(this KSqlFunctions kSqlFunctions, double[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Given an array, return the number of elements in the array. If the array field is NULL, or contains only NULLs, then NULL is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="array">The array</param>
    /// <returns>Returns the maximum value from within a given array of primitive elements (not arrays of other arrays, or maps, or structs, or combinations thereof).</returns>
    public static int? ArrayLength(this KSqlFunctions kSqlFunctions, decimal[] array)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region ARRAY_MAX

    

    #endregion

    #endregion

    #region String functions

    //TODO: move to query functions
    public static bool Like(this KSqlFunctions kSqlFunctions, string condition, string patternString)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Pads the input string, beginning from the left, with the specified padding string until the target length
    /// is reached. If the input string is longer than the specified target length it will be truncated. If the
    /// padding string is empty or NULL, or the target length is negative, then NULL is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input">String to be padded</param>
    /// <param name="length">Target length</param>
    /// <param name="padding">Padding string</param>
    /// <returns>Padded string</returns>
    public static string LPad(this KSqlFunctions kSqlFunctions, string input, int length, string padding)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Pads the input string, starting from the end, with the specified padding string until the target length is
    /// reached. If the input string is longer than the specified target length it will be truncated. If the
    /// padding string is empty or NULL, or the target length is negative, then NULL is returned.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input">String to be padded</param>
    /// <param name="length">Target length</param>
    /// <param name="padding">Padding string</param>
    /// <returns>Padded string</returns>
    public static string RPad(this KSqlFunctions kSqlFunctions, string input, int length, string padding)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns a substring of str that starts at pos and is of length len
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input">The source string</param>
    /// <param name="position">The base-one position to start from</param>
    /// <param name="length">The length to extract</param>
    /// <returns></returns>
    public static string Substring(this KSqlFunctions kSqlFunctions, string input, int position, int length)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Returns a substring of str from pos to the end of str
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input">The source string</param>
    /// <param name="position">The base-one position to start from.</param>
    /// <returns></returns>
    public static string Substring(this KSqlFunctions kSqlFunctions, string input, int position)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Remove whitespace from the beginning and end of a string.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="input">The string to trim</param>
    /// <returns>Trimmed string</returns>
    public static string Trim(this KSqlFunctions kSqlFunctions, string input)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion

    #region Date and time functions
    
    /// <summary>
    /// Gets an integer representing days since epoch.
    /// </summary>
    /// <returns></returns>
    public static int UnixDate(this KSqlFunctions kSqlFunctions)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Gets the Unix timestamp in milliseconds, represented as a long (BIGINT).
    /// </summary>
    /// <returns></returns>
    public static long UnixTimestamp(this KSqlFunctions kSqlFunctions)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Converts an integer representing days since epoch to a date string using the given format pattern.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="epochDays">The Epoch Day to convert, based on the epoch 1970-01-01</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns></returns>
    public static string DateToString(this KSqlFunctions kSqlFunctions, int epochDays, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Converts a string representation of a date into an integer representing days since epoch using the given format pattern.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="formattedDate">The string representation of a date</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns></returns>
    public static int StringToDate(this KSqlFunctions kSqlFunctions, string formattedDate, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Converts a string representation of a date in the given format into the BIGINT value that represents the millisecond timestamp.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="formattedTimestamp">The string representation of a date.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns></returns>
    public static long StringToTimestamp(this KSqlFunctions kSqlFunctions, string formattedTimestamp, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Converts a string representation of a date in the given format into the BIGINT value that represents the millisecond timestamp.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="formattedTimestamp">The string representation of a date.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <param name="timeZone">timeZone is a java.util.TimeZone ID format, for example: "UTC", "America/Los_Angeles", "PST", "Europe/London"</param>
    /// <returns></returns>
    public static long StringToTimestamp(this KSqlFunctions kSqlFunctions, string formattedTimestamp, string formatPattern, string timeZone)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }
    
    /// <summary>
    /// Converts a BIGINT millisecond timestamp value into the string representation of the timestamp in the given format.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="epochMilli">Milliseconds since January 1, 1970, 00:00:00 GMT.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <returns>String representation of the timestamp in the given format.</returns>
    public static string TimestampToString(this KSqlFunctions kSqlFunctions, long epochMilli, string formatPattern)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    /// <summary>
    /// Converts a BIGINT millisecond timestamp value into the string representation of the timestamp in the given format.
    /// </summary>
    /// <param name="kSqlFunctions"></param>
    /// <param name="epochMilli">Milliseconds since January 1, 1970, 00:00:00 GMT.</param>
    /// <param name="formatPattern">The format pattern should be in the format expected by java.time.format.DateTimeFormatter.</param>
    /// <param name="timeZone">timeZone is a java.util.TimeZone ID format, for example: "UTC", "America/Los_Angeles", "PST", "Europe/London"</param>
    /// <returns>String representation of the timestamp in the given format.</returns>
    public static string TimestampToString(this KSqlFunctions kSqlFunctions, long epochMilli, string formatPattern, string timeZone)
    {
      throw new InvalidOperationException(ServerSideOperationErrorMessage);
    }

    #endregion
  }
}