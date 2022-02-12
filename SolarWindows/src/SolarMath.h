//===============================================================//
/*
	____                        _
  / ____|                      (_)
 | |      ___   ___  _ __ ___   _   ___
 | |     / _ \ / __|| '_ ` _ \ | | / __|
 | |____| (_) |\__ \| | | | | || || (__
  \_____|\___/ |___/|_| |_| |_||_| \___|

*/
//===============================================================//

#pragma once
#include "Core.h"

#pragma warning( push )
#pragma warning( disable : 4251 )


//************************************
// Defines
//************************************
#define PI 3.14159265359f
#define FLOATING_POINT_ERROR_PRESCION 0.0001f

//************************************
// Standard functions
//************************************

inline constexpr uint32 SafeTruncateUint64(uint64 value)
{
	Assert(value <= 0xFFFFFFFF, "Value to high to 32 bit");

	uint32 result = static_cast<uint32>(value);

	return result;
}

inline constexpr real32 SafeTruncateDouble(real64 value)
{
	Assert(value <= 0xFFFFFFFF, "Value to high to 32 bit");

	real32 result = static_cast<real32>(value);

	return result;
}

template<typename T>
inline constexpr T Abs(const T& t)
{
	return t < static_cast<T>(0.0) ? static_cast<T>(-1.0) * t : t;
}

real64  RandomReal64();
real32  RandomReal32();

real64  RandomReal64(real64 min, real64 max);
real32  RandomReal32(real32 min, real32 max);

int64   RandomInt64(int64 min, int64 max);
int32   RandomInt32(int32 min, int32 max);

real32  Sin(const real32& rad);
real64  Sin(const real64& rad);

real32  ArcSin(const real32& rad);
real64  ArcSin(const real64& rad);

real32  Cos(const real32& rad);
real64  Cos(const real64& rad);

real32  ArcCos(const real32& rad);
real64  ArcCos(const real64& rad);

real32  Tan(const real32& rad);
real64  Tan(const real64& rad);

real32  ATan2(const real32& y, const real32& x);
real64  ATan2(const real64& y, const real64& x);

real32  Round(const real32& x);
real64  Round(const real64& x);

real32  Floor(const real32& x);
real64  Floor(const real64& x);

real32  Ceil(const real32& x);
real64  Ceil(const real64& x);

real32  Pow(const real32& base, const real32& exp);
real64  Pow(const real64& base, const real64& exp);

real32  Sqrt(const real32& x);
real64  Sqrt(const real64& x);

template <typename T>
inline constexpr T Normalize(const T& value, const T& min, const T& max)
{
	return (value - min) / (max - min);
}

template <typename T>
inline constexpr T Lerp(const T& a, const T& b, const T& t)
{
	return a * (static_cast<T>(1.0) - t) + b * t;
}

template <typename T>
inline constexpr T DegToRad(const T& degrees)
{
	return (static_cast<T>(PI) * degrees) / static_cast<T>(180.0);
}

template <typename T>
inline constexpr T RadToDeg(const T& radians)
{
	return (static_cast<T>(180.0) * radians) / static_cast<T>(PI);
}

template <typename T>
inline constexpr T Sign(const T& val)
{
	if (val > static_cast<T>(0.0)) { return static_cast<T>(1.0); }
	if (val < static_cast<T>(0.0)) { return static_cast<T>(-1.0); }
	return static_cast<T>(0.0);
}

template <typename T>
inline constexpr void Swap(T* a1, T* a2)
{
	T temp = *a1;
	*a1 = *a2;
	*a2 = temp;
}

template <typename T>
inline constexpr bool32 Equal(const T& a, const T& b, const T& epsilon = FLOATING_POINT_ERROR_PRESCION)
{
	return (Abs(a - b) < epsilon) ? true : false;
}

template <typename T>
inline constexpr bool32 Nequal(const T& a, const T& b, const T& epsilon = FLOATING_POINT_ERROR_PRESCION)
{
	return !Equal(a, b);
}

template <typename T>
inline constexpr T Clamp(const T& value, const T& lowerBound, const T& upperBound)
{
	T result = value < lowerBound ? lowerBound : value;
	result = value > upperBound ? upperBound : value;

	return result;
}

template <typename T>
inline constexpr T Min(const T& f)
{
	return f;
}

template <typename T, typename... Args>
inline constexpr T Min(const T& f, const Args &... args)
{
	T min = Min(args...);

	return (f < min) ? f : min;
}

template <typename T>
inline constexpr T SmoothMin(const T& a, const T& b, const T& t)
{
	// @HELP: http://www.viniciusgraciano.com/blog/smin/
	Assert(!Equal(t, static_cast<T>(0)), "Math error");

	T h = Clamp(static_cast<T>(0.5) + static_cast<T>(0.5) * (a - b) / t, static_cast<T>(0.0), static_cast<T>(1.0));
	T result = a * h + b * (1 - h) - t * h * (1 - h);

	return result;
}

template <typename T>
inline constexpr T Max(const T& f)
{
	return f;
}

template <typename T, typename... Args>
inline constexpr T Max(const T& f, const Args &... args)
{
	T max = Max(args...);

	return (f > max) ? f : max;
}

template<typename T>
inline constexpr T SmoothMax(const T& a, const T& b, const T& t)
{
	// @HELP: http://www.viniciusgraciano.com/blog/smin/
	Assert(!Equal(t, static_cast<T>(0)), "Math error");

	T h = Clamp(static_cast<T>(0.5) + static_cast<T>(0.5) * (b - a) / t, static_cast<T>(0.0), static_cast<T>(1.0));
	T result = a * h + b * (1 - h) + t * h * (1 - h);

	return result;
}

template<typename T>
inline constexpr T Step(const T& a, const T& edge)
{
	return a < edge ? static_cast<T>(0.0) : static_cast<T>(1.0);
}

template<typename T>
inline constexpr T SmoothStep(const T& t)
{
	T result = t * t * (static_cast<T>(3.0) - static_cast<T>(2.0) * t);

	return result;
}

template <typename T>
inline constexpr T SmoothStep(const T& t, const T& lower, const T& upper)
{
	T norm = (t - lower) / (upper - lower);
	norm = Clamp(norm, lower, upper);

	T result = norm * norm * (static_cast<T>(3.0) - static_cast<T>(2.0) * norm);

	return result;
}

template <typename T>
inline constexpr T SmootherStep(const T& t)
{
	T result = t * t * t * (t * (t * static_cast<T>(6.0) - static_cast<T>(15)) + static_cast<T>(10.0));

	return result;
}

template <typename T>
inline constexpr T SmootherStep(const T& t, const T& lower, const T& upper)
{
	T norm = (t - lower) / (upper - lower);
	norm = Clamp(norm, lower, upper);

	T result = norm * norm * norm * (norm * (norm * static_cast<T>(6.0) - static_cast<T>(15)) + static_cast<T>(10.0));

	return result;
}

template <typename T>
inline constexpr T InverseSmoothStep(const T& t)
{
	T result = static_cast<T>(0.5) - Sin(Asin(static_cast<T>(1.0) - static_cast<T>(2.0) * t) / static_cast<T>(3.0));

	return result;
}

template <typename T>
inline constexpr T InverseSmoothStep(const T& t, const T& lower, const T upper)
{
	T norm = (t - lower) / (upper - lower);
	norm = Clamp(norm, lower, upper);

	T result = static_cast<T>(0.5) - Sin(Asin(static_cast<T>(1.0) - static_cast<T>(2.0) * norm) / static_cast<T>(3.0));

	return result;
}

template <typename T>
inline constexpr T RadicalInverse(uint32 a)
{
	T result = static_cast<T>(0.0);
	T f = static_cast<T>(0.5);

	while (a)
	{
		result += f * static_cast<T>((!a & 1));
		a /= static_cast<T>(2);
		f *= static_cast<T>(0.5);
	}

	return result;
}

template <typename T>
inline constexpr T BiasFunction(const T& a, const T& bias)
{
	T m = static_cast<T>(1) - bias;
	T t = m * m * m;

	T result = (a * t) / (a * t - a + 1);

	return result;
}

//************************************
// Polar Coord
//************************************

template <typename T>
struct PolarCoord
{
	T r;
	T theta;
	T z;
};

//************************************
// Vector2
//************************************

template <typename T>
struct Vec2
{
	union
	{
		T ptr[2];

		struct
		{
			T x;
			T y;
		};
	};

	Vec2()
	{
		this->x = static_cast<T>(0);
		this->y = static_cast<T>(0);
	}

	Vec2(const T& x, const T& y)
	{
		this->x = x;
		this->y = y;
	}

	Vec2(const T& all)
	{
		this->x = all;
		this->y = all;
	}

	//explicit Vec2(String str)
	//{
	//	str.RemoveCharacter(0);
	//	str.RemoveCharacter(str.GetLength() - 1);

	//	ManagedArray<String> values = str.Split(';');

	//	this->x = static_cast<T>(values[0].ToReal32());
	//	this->y = static_cast<T>(values[1].ToReal32());
	//}

	//inline String ToString()
	//{
	//	String result = "";

	//	result.Add('(').Add(x).Add(";").Add(y).Add(')');

	//	return result;
	//}

	T& operator[](const int32& index)
	{
		Assert(index >= 0 && index < 2, "Math error");

		return (&x)[index];
	}

	T operator[](const int32& index) const
	{
		Assert(index >= 0 && index < 2, "Math error");

		return (&x)[index];
	}
};

typedef Vec2<real32> Vec2f;
typedef Vec2<int32> Vec2i;
typedef Vec2<double> Vec2d;

template <typename T>
inline T MagSqrd(const Vec2<T>& a)
{
	T result = a.x * a.x + a.y * a.y;

	return result;
}

template <typename T>
inline T Mag(const Vec2<T>& a)
{
	T result = Sqrt(a.x * a.x + a.y * a.y);

	return result;
}

template <typename T>
inline Vec2<T> Normalize(const Vec2<T>& a)
{
	T magA = Mag(a);

	if (Equal(magA, static_cast<T>(0.0)))
		return Vec2<T>();

	Vec2<T> result = a / magA;

	return result;
}

template <typename T>
inline T Dot(const Vec2<T>& a, const Vec2<T>& b)
{
	T result = a.x * b.x + a.y * b.y;

	return result;
}

template <typename T>
inline T Distance(const Vec2<T>& a, const Vec2<T>& b)
{
	T result = Mag(a - b);

	return result;
}

template <typename T>
inline T DistanceSqrd(const Vec2<T>& a, const Vec2<T>& b)
{
	T result = (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);

	return result;
}

template <typename T>
inline Vec2<T> Abs(const Vec2<T>& a)
{
	Vec2<T> result = Vec2<T>(Abs(a.x), Abs(a.y));

	return result;
}

template <typename T>
inline Vec2<T> Clamp(const Vec2<T>& value, const Vec2<T>& lower_bound, const Vec2<T>& upper_bound)
{
	T x = Clamp(value.x, lower_bound.x, upper_bound.x);
	T y = Clamp(value.y, lower_bound.y, upper_bound.y);

	Vec2<T> result = Vec2<T>(x, y);

	return result;
}

template <typename T>
inline bool32 Equal(const Vec2<T>& a1, const Vec2<T> a2, const real32& epsilon = FLOATING_POINT_ERROR_PRESCION)
{
	T dx = Abs(a1.x - a2.x);
	T dy = Abs(a1.y - a2.y);

	bool32 result = (dx < epsilon&& dy < epsilon);

	return result;
}

template <typename T>
inline constexpr Vec2<T> operator+(const Vec2<T>& a, const Vec2<T>& b)
{
	Vec2<T> result = Vec2<T>(a.x + b.x, a.y + b.y);

	return result;
}

template <typename T>
inline constexpr Vec2<T> operator-(const Vec2<T>& a, const Vec2<T>& b)
{
	Vec2<T> result = Vec2<T>(a.x - b.x, a.y - b.y);

	return result;
}

template <typename T>
inline constexpr void operator*=(Vec2<T>& a, const real32& b)
{
	a = Vec2<T>(a.x * b, a.y * b);
}

template <typename T>
inline constexpr void operator/=(Vec2<T>& a, const real32& b)
{
	a = Vec2<T>(a.x / b, a.y / b);
}

template <typename T>
inline constexpr void operator+=(Vec2<T>& a, const Vec2<T>& b)
{
	a = Vec2<T>(a.x + b.x, a.y + b.y);
}

template <typename T>
inline constexpr void operator-=(Vec2<T>& a, const Vec2<T>& b)
{
	a = Vec2<T>(a.x - b.x, a.y - b.y);
}

template <typename T>
inline constexpr void operator*=(Vec2<T>& a, const Vec2<T>& b)
{
	a = Vec2<T>(a.x * b.x, a.y * b.y);
}

template <typename T>
inline constexpr void operator/=(Vec2<T>& a, const Vec2<T>& b)
{
	a = Vec2<T>(a.x / b.x, a.y / b.y);
}

template <typename T>
inline constexpr Vec2<T> operator*(const Vec2<T>& a, const T& b)
{
	Vec2<T> result = Vec2<T>(a.x * b, a.y * b);

	return result;
}

template <typename T>
inline constexpr Vec2<T> operator*(const T& a, const Vec2<T>& b)
{
	Vec2<T> result = Vec2<T>(a * b.x, a * b.y);

	return result;
}

template <typename T>
inline constexpr Vec2<T> operator/(const Vec2<T>& a, const T& b)
{
	Vec2<T> result = Vec2<T>(a.x / b, a.y / b);

	return result;
}


template <typename T>
inline constexpr Vec2<T> operator*(const Vec2<T>& a, const Vec2<T>& b)
{
	Vec2<T> result = Vec2<T>(a.x * b.x, a.y * b.y);

	return result;
}

template <typename T>
inline constexpr Vec2<T> operator/(const Vec2<T>& a, const Vec2<T>& b)
{
	Vec2<T> result = Vec2<T>(a.x / b.x, a.y / b.y);

	return result;
}

template <typename T>
inline constexpr bool32 operator==(const Vec2<T>& a, const Vec2<T>& b)
{
	bool32 result = (a.x == b.x && a.y == b.y);

	return result;
}

template <typename T>
inline constexpr bool32 operator!=(const Vec2<T>& a, const Vec2<T>& b)
{
	return (a.x != b.x) || (a.y != b.y);
}

//************************************
// Vector3
//************************************

template <typename T>
struct Vec3
{
	union
	{
		T ptr[3];
		struct
		{
			union
			{
				T x;
				T r;
			};
			union
			{
				T y;
				T g;
			};
			union
			{
				T z;
				T b;
			};
		};
	};

	Vec3()
	{
		this->x = static_cast<T>(0.0);
		this->y = static_cast<T>(0.0);
		this->z = static_cast<T>(0.0);
	}

	Vec3(const T& x, const T& y, const T& z)
	{
		this->x = x;
		this->y = y;
		this->z = z;

	}

	Vec3(const T& all)
	{
		this->x = all;
		this->y = all;
		this->z = all;
	}

	//explicit Vec3(String s)
	//{
	//	s.RemoveCharacter(0);
	//	s.RemoveCharacter(s.GetLength() - 1);

	//	ManagedArray<String>values = s.Split(';');

	//	this->x = values[0].ToReal32();
	//	this->y = values[1].ToReal32();
	//	this->z = values[2].ToReal32();
	//}

	//inline String ToString()
	//{
	//	String result = "";

	//	result.Add('(').Add((real32)x).Add(";").Add((real32)y).Add(";").Add((real32)z).Add(')');

	//	return result;
	//}

	T& operator[](const int32& index)
	{
		Assert(index >= 0 && index < 3, "Vec3 [] operator, invalid index");

		return (&x)[index];
	}

	T operator[](const int32& index) const
	{
		Assert(index >= 0 && index < 3, "Vec3 [] operator, invalid index");

		return (&x)[index];
	}
};

typedef Vec3<real32> Vec3f;
typedef Vec3<real64> Vec3d;
typedef Vec3<int32> Vec3i;

template <typename T>
inline T Dot(const Vec3<T>& a, const Vec3<T>& b)
{
	T result = a.x * b.x + a.y * b.y + a.z * b.z;

	return result;
}

template <typename T>
inline T Mag(const Vec3<T>& a)
{
	T result = Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);

	return result;
}

template <typename T>
inline T MagSqrd(const Vec3<T>& a)
{
	T result = (a.x * a.x + a.y * a.y + a.z * a.z);

	return result;
}

template <typename T>
inline Vec3<T> ClampMag(const Vec3<T>& v, const T& m)
{
	Assert(m >= static_cast<T>(0.0f), "ClampMag: m was less than zero");

	T mag2 = MagSqrd(v);
	T m2 = m * m;

	if (mag2 > m2)
	{
		real32 s = m / Sqrt(mag2);
		return v * s;
	}

	return v;
}

template <typename T>
inline T Distance(const Vec3<T>& a, const Vec3<T>& b)
{
	T result = Mag(a - b);

	return result;
}

template <typename T>
inline T DistanceSqrd(const Vec3<T>& a, const Vec3<T>& b)
{
	T result = (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);

	return result;
}

template <typename T>
inline int32 MaxIndex(const Vec3<T>& a)
{
	int32 result = (a.x > a.y) ? ((a.x > a.z) ? 0 : 2) : ((a.y > a.z) ? 1 : 2);

	return result;
}

template <typename T>
inline int32 AbsMaxIndex(const Vec3<T>& a)
{
	T x = Abs(a.x);
	T y = Abs(a.y);
	T z = Abs(a.z);

	int32 result = (x > y) ? ((x > z) ? 0 : 2) : ((y > z) ? 1 : 2);

	return result;
}

template <typename T>
inline Vec3<T> Normalize(const Vec3<T>& a)
{
	T m = Mag(a);
	T invMag = static_cast<T>(1.0) / m;

	Vec3<T> result = a;
	if (0.0f * invMag == 0.0f * invMag) {
		result = Vec3<T>(a.x * invMag, a.y * invMag, a.z * invMag);
	}

	return result;
}

template <typename T>
inline Vec3<T> Cross(const Vec3<T>& a, const Vec3<T>& b)
{
	T x = a.y * b.z - b.y * a.z;
	T y = a.z * b.x - b.z * a.x;
	T z = a.x * b.y - b.x * a.y;

	Vec3<T> result = Vec3<T>(x, y, z);

	return result;
}

template <typename T>
inline Vec3<T> Lerp(const Vec3<T>& a, const Vec3<T>& b, const real32& t)
{
	Vec3<T> result = Vec3<T>(
		a.x + t * (b.x - a.x),
		a.y + t * (b.y - a.y),
		a.z + t * (b.z - a.z));

	return result;
}

template <typename T>
inline Vec3<T> Reflect(const Vec3<T>& r, const Vec3<T>& normal)
{
	T d = Dot(r, normal);

	Vec3<T> result = r - static_cast<T>(2.0) * d * normal;

	return result;
}

template <typename T>
inline Vec3<T> Refract(const Vec3<T>& uv, const Vec3<T>& n, const T& etai_over_etat)
{
	auto cos_theta = Min(Dot(static_cast<T>(-1.0) * uv, n), static_cast<T>(1.0));
	Vec3<T> r_out_perp = etai_over_etat * (uv + cos_theta * n);
	Vec3<T> r_out_parallel = -Sqrt(Abs(static_cast<T>(1.0) - MagSqrd(r_out_perp))) * n;
	Vec3<T> result = r_out_perp + r_out_parallel;

	return result;
}

template <typename T>
inline Vec3<T> Project(const Vec3<T>& a, const Vec3<T>& b) // @NOTE: Projects a onto b
{
	T nume = Dot(a, b);
	T demon = MagSqrd(b);

	T s = nume / demon;

	Vec3<T> result = s * b;

	return result;
}

template <typename T>
inline Vec3<T> Clamp(const Vec3<T>& value, const Vec3<T>& lower_bound, const Vec3<T>& upper_bound)
{
	T x = Clamp(value.x, lower_bound.x, upper_bound.x);
	T y = Clamp(value.y, lower_bound.y, upper_bound.y);
	T z = Clamp(value.z, lower_bound.z, upper_bound.z);

	Vec3<T> result = Vec3<T>(x, y, z);

	return result;
}

template<typename T>
inline Vec3<T> Min(const Vec3<T>& a, const Vec3<T>& b)
{
	Vec3<T> result = {};

	result.x = Min(a.x, b.x);
	result.y = Min(a.y, b.y);
	result.z = Min(a.z, b.z);

	return result;
}

template<typename T>
inline Vec3<T> Sign(const Vec3<T>& a)
{
	Vec3<T> result = {};

	result.x = Sign(a.x);
	result.y = Sign(a.y);
	result.z = Sign(a.z);

	return result;
}

template<typename T>
inline Vec3<T> Step(const Vec3<T>& a, const Vec3<T>& edge)
{
	Vec3<T> result = {};

	result.x = Step(a.x, edge.x);
	result.y = Step(a.y, edge.y);
	result.z = Step(a.z, edge.z);

	return result;
}

template<typename T>
inline Vec3<T> Max(const Vec3<T>& a, const Vec3<T>& b)
{
	Vec3<T> result = {};

	result.x = Max(a.x, b.x);
	result.y = Max(a.y, b.y);
	result.z = Max(a.z, b.z);

	return result;
}

template <typename T>
inline Vec3<T> Abs(const Vec3<T>& a)
{
	Vec3<T> result = Vec3<T>(Abs(a.x), Abs(a.y), Abs(a.z));

	return result;
}

template <typename T>
bool32 Equal(const Vec3<T>& a, const Vec3<T>& b, const T& epsilon = FLOATING_POINT_ERROR_PRESCION)
{
	bool32 resx = Equal(a.x, b.x, epsilon);
	bool32 resy = Equal(a.y, b.y, epsilon);
	bool32 resz = Equal(a.z, b.z, epsilon);

	bool32 result = resx && resy && resz;

	return result;
}

template <typename T>
inline constexpr bool32 operator==(const Vec3<T>& a, const Vec3<T>& b)
{
	return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
}

template <typename T>
inline constexpr bool32 operator!=(const Vec3<T>& a, const Vec3<T>& b)
{
	return (a.x != b.x) || (a.y != b.y) || (a.z != b.z);
}

template <typename T>
inline constexpr Vec3<T> operator+(const Vec3<T>& a, const Vec3<T>& b)
{
	return Vec3<T>(a.x + b.x, a.y + b.y, a.z + b.z);
}

template <typename T>
inline constexpr Vec3<T> operator-(const Vec3<T>& a, const Vec3<T>& b)
{
	return Vec3<T>(a.x - b.x, a.y - b.y, a.z - b.z);
}

template <typename T>
inline constexpr Vec3<T> operator*(const Vec3<T>& a, const Vec3<T>& b)
{
	return Vec3<T>(a.x * b.x, a.y * b.y, a.z * b.z);
}

template <typename T>
inline constexpr Vec3<T> operator/(const Vec3<T>& a, const Vec3<T>& b)
{
	return Vec3<T>(a.x / b.x, a.y / b.y, a.z / b.z);
}

template <typename T>
inline constexpr Vec3<T> operator*(const Vec3<T>& a, const T& b)
{
	return Vec3<T>(a.x * b, a.y * b, a.z * b);
}

template <typename T>
inline constexpr Vec3<T> operator*(const T& a, const Vec3<T>& b)
{
	return Vec3<T>(b.x * a, b.y * a, b.z * a);
}

#ifdef RELEASE
#pragma warning(push)
#pragma warning(disable : 4723)
#endif

template <typename T>
inline constexpr Vec3<T> operator/(const Vec3<T>& a, const T& b)
{
	//Assert(a.x != 0 && a.y != 0 && a.z != 0);
	return Vec3<T>(a.x / b, a.y / b, a.z / b);
}

template <typename T>
inline constexpr Vec3<T> operator/(const T& b, const Vec3<T>& a)
{
	//Assert(a.x != 0 && a.y != 0 && a.z != 0);
	return Vec3<T>(b / a.x, b / a.y, b / a.z);
}

#ifdef RELEASE
#pragma warning(pop)
#endif

template <typename T>
inline constexpr void operator+=(Vec3<T>& a, const Vec3<T>& b)
{
	a = Vec3<T>(a.x + b.x, a.y + b.y, a.z + b.z);
}

template <typename T>
inline constexpr void operator-=(Vec3<T>& a, const Vec3<T>& b)
{
	a = Vec3<T>(a.x - b.x, a.y - b.y, a.z - b.z);
}

//************************************
// Vector4
//************************************

template <typename T>
struct Vec4
{
	union
	{
		T ptr[4];
		struct
		{
			union
			{
				T x;
				T r;
			};
			union
			{
				T y;
				T g;
			};
			union
			{
				T z;
				T b;
			};
			union
			{
				T w;
				T a;
			};
		};
	};

	Vec4()
	{
		this->x = 0;
		this->y = 0;
		this->z = 0;
		this->w = 0;
	}

	Vec4(const T& x, const T& y, const T& z, const T& w)
	{
		this->x = x;
		this->y = y;
		this->z = z;
		this->w = w;
	}

	Vec4(const T& all)
	{
		this->x = all;
		this->y = all;
		this->z = all;
		this->w = all;
	}

	Vec4(const Vec3<T>& xyz, const T& w)
	{
		this->x = xyz.x;
		this->y = xyz.y;
		this->z = xyz.z;
		this->w = w;
	}

	//Vec4(String str)
	//{
	//	str.RemoveCharacter(0);
	//	str.RemoveCharacter(str.GetLength() - 1);

	//	ManagedArray<String>values = str.Split(';');
	//	this->x = static_cast<T>(values[0].ToReal32());
	//	this->y = static_cast<T>(values[1].ToReal32());
	//	this->z = static_cast<T>(values[2].ToReal32());
	//	this->w = static_cast<T>(values[3].ToReal32());
	//}

	T& operator[](const int32& index)
	{
		Assert(index >= 0 && index < 4, "Vec4 [] operator, invalid index");
		return (&x)[index];
	}

	T operator[](const int32& index) const
	{
		Assert(index >= 0 && index < 4, "Vec4 [] operator, invalid index");
		return (&x)[index];
	}
};

typedef Vec4<real32> Vec4f;
typedef Vec4<real64> Vec4d;
typedef Vec4<int32> Vec4i;

template<typename T>
Vec3<T> Vec4ToVec3(const Vec4<T>& a)
{
	Vec3<T> r = {};
	r.x = a.x;
	r.y = a.y;
	r.z = a.z;

	return r;
}

template <typename T>
inline T Mag(const Vec4<T>& a)
{
	return Sqrt(a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w);
}

template <typename T>
inline Vec4<T> Normalize(const Vec4<T>& a)
{
	T m = Mag(a);

	if (Equal(m, 0.0))
		return Vec4<T>();

	Vec4<T> result = Vec4<T>(a.x / m, a.y / m, a.z / m, a.w / m);

	return result;
}

template <typename T>
inline T Dot(const Vec4<T>& a, const Vec4<T>& b)
{
	return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
}

//template <typename T>
//inline String ToString(const Vec4<T>& a)
//{
//	String result;
//
//	result.Add('(').Add(a.x).Add(";").Add(a.y).Add(";").Add(a.z).Add(";").Add(a.w).Add(')');
//
//	return result;
//}

template <typename T>
inline constexpr bool32 operator==(const Vec4<T>& a, const Vec4<T>& b)
{
	return (a.x == b.x) && (a.y == b.y) && (a.z == b.z) && (a.w == a.w);
}

template <typename T>
inline constexpr bool32 operator!=(const Vec4<T>& a, const Vec4<T>& b)
{
	return (a.x != b.x) || (a.y != b.y) || (a.z != b.z) || (a.w != b.w);
}

template <typename T>
inline constexpr Vec4<T> operator+(const Vec4<T>& a, const Vec4<T>& b)
{
	return Vec4<T>(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
}

template <typename T>
inline constexpr Vec4<T> operator-(const Vec4<T>& a, const Vec4<T>& b)
{
	return Vec4<T>(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
}

template <typename T>
inline constexpr Vec4<T> operator*(const Vec4<T>& a, const Vec4<T>& b)
{
	return Vec4<T>(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
}

template <typename T>
inline constexpr Vec4<T> operator*(const Vec4<T>& a, const T& b)
{
	return Vec4<T>(a.x * b, a.y * b, a.z * b, a.w * b);
}

#ifdef RELEASE
#pragma warning(push)
#pragma warning(disable : 4723)
#endif

template <typename T>
inline Vec4<T> operator/(const Vec4<T>& a, const Vec4<T>& b)
{
	//Assert(a.x != 0 && a.y != 0 && a.z != 0);
	return Vec4<T>(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
}

template <typename T>
inline Vec4<T> operator/(const Vec4<T>& a, const T& b)
{
	//Assert(a.x != 0 && a.y != 0 && a.z != 0);
	return Vec4<T>(a.x / b, a.y / b, a.z / b, a.w / b);
}

#ifdef RELEASE
#pragma warning(pop)
#endif

//************************************
// Quaterion functions
//************************************

template <typename T>
struct Quat
{
	union
	{
		T ptr[4];
		struct
		{
			T x;
			T y;
			T z;
			T w;
		};
	};

	Quat()
	{
		this->x = static_cast<T>(0.0);
		this->y = static_cast<T>(0.0);
		this->z = static_cast<T>(0.0);
		this->w = static_cast<T>(1.0);
	}

	Quat(const T& x, const T& y, const T& z, const T& w)
	{
		this->x = x;
		this->y = y;
		this->z = z;
		this->w = w;
	}

	Quat(const Vec3f& xyz, const T& w)
	{
		this->x = xyz.x;
		this->y = xyz.y;
		this->z = xyz.z;
		this->w = w;
	}

	//explicit Quat(String s)
	//{
	//	s.RemoveCharacter(0);
	//	s.RemoveCharacter(s.GetLength() - 1);

	//	ManagedArray<String> temp = s.Split(' ');
	//	ManagedArray<String> values = temp[0].Split(';');

	//	this->x = static_cast<T>(values[0].ToReal32());
	//	this->y = static_cast<T>(values[1].ToReal32());
	//	this->z = static_cast<T>(values[2].ToReal32());
	//	this->w = static_cast<T>(temp[1].ToReal32());
	//}

	//inline String ToString()
	//{
	//	String result;

	//	result.Add('(').Add('{').Add(x).Add(";").Add(y).Add(";").Add(z).Add('}').Add(' ').Add(w).Add(')');

	//	return result;
	//}

	T& operator[](const int32& index)
	{
		Assert(index >= 0 && index < 4, "Math error");
		return (&x)[index];
	}

	T operator[](const int32& index) const
	{
		Assert(index >= 0 && index < 4, "Math error");
		return (&x)[index];
	}
};

typedef Quat<real32> Quatf;

template<typename T>
inline Quat<T> Conjugate(const Quat<T>& a)
{
	return Quat<T>(-a.x, -a.y, -a.z, a.w);
}

template<typename T>
inline T Mag(const Quat<T>& a)
{
	return Sqrt(a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w);
}

template<typename T>
inline T MagSqrd(const Quat<T>& a) {
	return ((a.x * a.x) + (a.y * a.y) + (a.z * a.z) + (a.w * a.w));
}

template<typename T>
inline Quat<T> Normalize(const Quat<T>& a)
{
	T invMag = static_cast<T>(1.0f) / Mag(a);
	if (0.0f * invMag == 0.0f * invMag) {
		return Quat<T>(a.x * invMag, a.y * invMag, a.z * invMag, a.w * invMag);
	}
	return a;
}

template<typename T>
Quat<T> EulerToQuat(const Vec3<T>& euler_angle)
{
	Vec3<T> c = Vec3f(Cos(DegToRad(euler_angle.x) / 2.0f), Cos(DegToRad(euler_angle.y) / 2.0f), Cos(DegToRad(euler_angle.z) / 2.0f));
	Vec3<T> s = Vec3f(Sin(DegToRad(euler_angle.x) / 2.0f), Sin(DegToRad(euler_angle.y) / 2.0f), Sin(DegToRad(euler_angle.z) / 2.0f));

	Quat<T> q;
	q.x = s.x * c.y * c.z - c.x * s.y * s.z;
	q.y = c.x * s.y * c.z + s.x * c.y * s.z;
	q.z = c.x * c.y * s.z - s.x * s.y * c.z;
	q.w = c.x * c.y * c.z + s.x * s.y * s.z;

	return q;
}

template<typename T>
Quat<T> EulerToQuat(const T& x, const T& y, const T& z)
{
	return EulerToQuat<T>(Vec3<T>(x, y, z));
}

template <typename T>
Vec3<T> QuatToEuler(const Quat<T>& q)
{
	// @HELP: Glm and Math book
	Vec3<T> euler;
	Vec2<T> sp;
	sp.x = 2.f * (q.y * q.z + q.w * q.x);
	sp.y = q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z;

	if (Equal(Vec2<T>(sp.x, sp.y), Vec2<T>(0.0)))
	{
		euler.x = 2.0f * ATan2(q.w, q.x);
	}
	else
	{
		euler.x = ATan2(sp.x, sp.y);
	}

	euler.y = ArcSin(Clamp(-2.0f * (q.x * q.z - q.w * q.y), -1.0f, 1.0f));
	euler.z = ATan2(2.0f * (q.x * q.y + q.w * q.z), q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);

	euler.x = RadToDeg(euler.x);
	euler.y = RadToDeg(euler.y);
	euler.z = RadToDeg(euler.z);

	return euler;
}

template<typename T>
Quat<T> GlobalRotateX(const T& rads, const Quat<T>& q)
{
	T sin = Sin(rads * static_cast<T>(0.5));
	T cos = Cos(rads * static_cast<T>(0.5));

	Quat<T> result = {};
	result.x = q.w * sin + q.x * cos;
	result.y = q.y * cos + q.z * sin;
	result.z = q.z * cos - q.y * sin;
	result.w = q.w * cos - q.x * sin;

	return result;
}

template<typename T>
Quat<T> GlobalRotateY(const T& rads, const Quat<T>& q)
{
	T sin = Sin(rads * static_cast<T>(0.5));
	T cos = Cos(rads * static_cast<T>(0.5));

	Quat<T> result = {};
	result.x = q.x * cos - q.z * sin;
	result.y = q.w * sin + q.y * cos;
	result.z = q.x * sin + q.z * cos;
	result.w = q.w * cos - q.y * sin;

	return result;
}

template<typename T>
Quat<T> GlobalRotateZ(const T& rads, const Quat<T>& q)
{
	T sin = Sin(rads * static_cast<T>(0.5));
	T cos = Cos(rads * static_cast<T>(0.5));

	Quat<T> result = {};
	result.x = q.x * cos + q.y * sin;
	result.y = q.y * cos - q.x * sin;
	result.z = q.w * sin + q.z * cos;
	result.w = q.w * cos - q.z * sin;

	return result;
}

template<typename T>
Quat<T> LocalRotateX(const T& rads, const Quat<T>& q)
{
	real32 hangle = rads * 0.5f;
	real32 s = Sin(hangle);
	real32 c = Cos(hangle);

	Quat<T> result;
	result.x = c * q.x + s * q.w;
	result.y = c * q.y - s * q.z;
	result.z = c * q.z + s * q.y;
	result.w = c * q.w - s * q.x;

	return result;
}

template<typename T>
Quat<T> LocalRotateY(const T& rads, const Quat<T>& q)
{
	real32 hangle = rads * 0.5f;
	real32 s = Sin(hangle);
	real32 c = Cos(hangle);

	Quat<T> result;
	result.x = c * q.x + s * q.z;
	result.y = c * q.y + s * q.w;
	result.z = c * q.z - s * q.x;
	result.w = c * q.w - s * q.y;

	return result;
}

template<typename T>
Quat<T> LocalRotateZ(const T& rads, const Quat<T>& q)
{
	real32 hangle = rads * 0.5f;
	real32 s = Sin(hangle);
	real32 c = Cos(hangle);

	Quat<T> result;
	result.x = c * q.x - s * q.y;
	result.y = c * q.y + s * q.x;
	result.z = c * q.z + s * q.w;
	result.w = c * q.w - s * q.z;

	return result;
}


template <typename T>
Vec3<T> RotatePointLHS(const Quat<T>& r, const Vec3<T>& point)
{
	//@Help: https://gamedev.stackexchange.com/questions/28395/rotating-vector3-by-a-quaternion
	Quat<T> rc = r;

	real32 t = 1.0f / MagSqrd(rc);
	rc.x = -rc.x * t;
	rc.y = -rc.y * t;
	rc.z = -rc.z * t;
	rc.w = rc.w * t;

	Quat<T> pp = Quat<T>(point, 0);
	Quat<T> res = r * pp * rc;

	return Vec3<T>(res.x, res.y, res.z);
}

template <typename T>
Vec3<T> RotatePointRHS(const Quat<T>& r, const Vec3<T>& point)
{
	//@Help: https://gamedev.stackexchange.com/questions/28395/rotating-vector3-by-a-quaternion
	Quat<T> rc = r;

	real32 t = 1.0f / MagSqrd(rc);
	rc.x = -rc.x * t;
	rc.y = -rc.y * t;
	rc.z = -rc.z * t;
	rc.w = rc.w * t;

	Quat<T> pp = Quat<T>(point, 0);
	Quat<T> res = rc * pp * r;

	return Vec3<T>(res.x, res.y, res.z);
}

template <typename T>
Vec3<T> RotatePointAboutAxis(const T& d_angle, const Vec3<T>& point, const Vec3<T>& axis)
{
	Vec3<T> ax = Normalize(axis);

	T sh = Sin(DegToRad(d_angle / 2));
	T ch = Cos(DegToRad(d_angle / 2));

	Quat<T> r(ax.x * sh,
		ax.y * sh,
		ax.z * sh,
		ch);

	Quat<T> rc = Conjugate(r);
	Quat<T> pp = Quat<T>(point, 0);

	Quat<T> res = (r * pp) * rc;

	return Vec3<T>(res.x, res.y, res.z);
}

template <typename T>
Quat<T> QuatFromAxisAngle(Vec3<T> axis, const T& angle_rads)
{
	// @HELP: https://stackoverflow.com/questions/4436764/rotating-a-quaternion-on-1-axis

	T s = Sin(angle_rads / static_cast <real32>(2.0));
	T c = Cos(angle_rads / static_cast <real32>(2.0));

	axis = Normalize(axis);

	T x = axis.x * s;
	T y = axis.y * s;
	T z = axis.z * s;
	T w = c;

	Quat<T> result(x, y, z, w);

	return result;
}

template <typename T>
Quat<T> RotateTo(const Vec3<T>& from, const Vec3<T>& to)
{
	Assert(0, "Untested math"); // @REASON: Untested!
	// @HELP: https://bitbucket.org/sinbad/ogre/src/9db75e3ba05c/OgreMain/include/OgreVector3.h#cl-651
	Quat<T> result;

	Vec3<T> a = Normalize(from);
	Vec3<T> b = Normalize(to);

	T d = Dot(a, b);

	if (d > static_cast<T>(1.0) - static_cast<T>(10.0f * FLOATING_POINT_ERROR_PRESCION))
	{
		// @NOTE: Parallel
		result = Quat<T>(0, 0, 0, 1);
	}
	else if (d < static_cast<T>(-1.0) + static_cast<T>(10.0f * FLOATING_POINT_ERROR_PRESCION))
	{
		// @NOTE: Parallel and pointing in opposite direction
		Vec3<T> axis = Cross(Vec3<T>(0, 1, 0), a);
		axis = Equal(Mag(axis), 0.0f) ? Cross(Vec3<T>(1, 0, 0), a) : axis;

		axis = Normalize(axis);

		result = Quat(axis, PI);
	}
	else
	{
		T s = Sqrt((static_cast<T>(1) + d) * static_cast<T>(2));
		T invs = static_cast<T>(1) / s;

		Vec3<T> c = Cross(a, b);

		result.x = c.x * invs;
		result.y = c.y * invs;
		result.z = c.z * invs;
		result.w = s * static_cast<T>(0.5);

		result = Normalize(result);
	}

	return result;
}

template <typename T>
Quat<T> Slerp(const Quat<T>& a, const Quat<T>& b, const T& t)
{
	Quat<T> an = a;
	Quat<T> bn = b;

	T d = an.x * bn.x + an.y * bn.y + an.z * bn.z + an.w * bn.w;
	T tinv = 1.0f - t;
	int32 ds = Sign(d);

	Quat<T> result;
	result.x = an.x * tinv + ds * t * bn.x;
	result.y = an.y * tinv + ds * t * bn.y;
	result.z = an.z * tinv + ds * t * bn.z;
	result.w = an.w * tinv + ds * t * bn.w;

	return Normalize(result);
}

template <typename T>
bool32 Equal(const Quat<T>& q1, const Quat<T>& q2, const T& epsilon = FLOATING_POINT_ERROR_PRESCION)
{
	T dx = Abs(q1.x - q2.x);
	T dy = Abs(q1.y - q2.y);
	T dz = Abs(q1.z - q2.z);
	T dw = Abs(q1.w - q2.w);
	return (dx < epsilon&& dy < epsilon&& dz < epsilon&& dw < epsilon);
}

template <typename T>
inline constexpr Quat<T> operator*(const Quat<T>& lhs, const Quat<T>& rhs)
{
	Quat<T> temp;
	temp.w = (lhs.w * rhs.w) - (lhs.x * rhs.x) - (lhs.y * rhs.y) - (lhs.z * rhs.z);
	temp.x = (lhs.x * rhs.w) + (lhs.w * rhs.x) + (lhs.y * rhs.z) - (lhs.z * rhs.y);
	temp.y = (lhs.y * rhs.w) + (lhs.w * rhs.y) + (lhs.z * rhs.x) - (lhs.x * rhs.z);
	temp.z = (lhs.z * rhs.w) + (lhs.w * rhs.z) + (lhs.x * rhs.y) - (lhs.y * rhs.x);
	return temp;
}

//************************************
// Matrix 2x2
//************************************

template <typename T>
struct Mat2
{
	union
	{
		T ptr[4];
		struct
		{
			Vec2<T> row0;
			Vec2<T> row1;
		};
	};

	Mat2()
	{
		this->row0 = Vec2<T>(static_cast<T>(1.0), static_cast<T>(0.0));
		this->row1 = Vec2<T>(static_cast<T>(0.0), static_cast<T>(1.0));
	}

	Mat2(const T& a)
	{
		this->row0 = Vec2<T>(static_cast<T>(a), static_cast<T>(0.0));
		this->row1 = Vec2<T>(static_cast<T>(0.0), static_cast<T>(a));
	}

	Mat2(const Vec2<T>& row0, const Vec2<T>& row1)
	{
		this->row0 = row0;
		this->row1 = row1;
	}

	Vec2<T>& operator[](const int32& index)
	{
		Assert(index >= 0 && index < 2, "Math error");
		return (&row0)[index];
	}

	Vec2<T> operator[](const int32& index) const
	{
		Assert(index >= 0 && index < 2, "Math error");
		return (&row0)[index];
	}
};

typedef Mat2<real32> Mat2f;

template <typename T>
inline constexpr T Det(const Mat2<T>& a)
{
	T result = a.ptr[0] * a.ptr[3] - a.ptr[1] * a.ptr[2];

	return result;
}

template <typename T>
Mat2<T> Rotate(const Mat2<T>& a, const T& d_angle)
{
	real32 rads = DegToRad(-d_angle);

	real32 ctheta = Cos(rads);
	real32 stheta = Sin(rads);

	Mat2f result;

	result.ptr[0] = ctheta;
	result.ptr[1] = -stheta;
	result.ptr[2] = stheta;
	result.ptr[3] = ctheta;

	return result * a;
}

//template <typename T>
//inline String ToString(const Mat2<T>& a)
//{
//	String result;
//
//	result.Add('(');
//	result.Add(ToString(a.row0));
//	result.Add(ToString(a.row1));
//	result.Add(')');
//
//	return result;
//}

template <typename T>
inline constexpr Mat2<T> operator*(const Mat2<T>& a, const Mat2<T>& b)
{
	Mat2f result;

	result.ptr[0] = a.ptr[1] * b.ptr[2] + a.ptr[0] * b.ptr[0];
	result.ptr[1] = a.ptr[0] * b.ptr[1] + a.ptr[1] * b.ptr[3];
	result.ptr[2] = a.ptr[3] * b.ptr[2] + a.ptr[2] * b.ptr[0];
	result.ptr[3] = a.ptr[2] * b.ptr[1] + a.ptr[3] * b.ptr[3];

	return result;
}

//************************************
// Matrix 3x3
//************************************

template <typename T>
struct Mat3
{
	union
	{
		T ptr[9];
		struct
		{
			Vec3<T> row0;
			Vec3<T> row1;
			Vec3<T> row2;
		};
	};

	Mat3()
	{
		this->row0 = Vec3<T>(static_cast<T>(1.0), static_cast<T>(0.0), static_cast<T>(0.0));
		this->row1 = Vec3<T>(static_cast<T>(0.0), static_cast<T>(1.0), static_cast<T>(0.0));
		this->row2 = Vec3<T>(static_cast<T>(0.0), static_cast<T>(0.0), static_cast<T>(1.0));
	}

	Mat3(const T& a)
	{
		this->row0 = Vec3<T>(static_cast<T>(a), static_cast<T>(0.0), static_cast<T>(0.0));
		this->row1 = Vec3<T>(static_cast<T>(0.0), static_cast<T>(a), static_cast<T>(0.0));
		this->row2 = Vec3<T>(static_cast<T>(0.0), static_cast<T>(0.0), static_cast<T>(a));
	}

	Mat3(const Vec3<T>& row0, const Vec3<T>& row1, const Vec3<T>& row2)
	{
		this->row0 = row0;
		this->row1 = row1;
		this->row2 = row2;
	}

	Mat3(const Mat2<T>& a, const Vec2<T>& translation)
	{
		this->row0 = Vec3<T>(a.row0, 0);
		this->row1 = Vec3<T>(a.row1, 0);
		this->row2 = Vec3<T>(translation, 1);
	}

	Vec3<T>& operator[](const int32& index)
	{
		Assert(index >= 0 && index < 3, "Vec3f [] operator, invalid index");
		return (&row0)[index];
	}

	Vec3<T> operator[](const int32& index) const
	{
		Assert(index >= 0 && index < 3, "Vec3f [] operator, invalid index");
		return (&row0)[index];
	}
};

typedef Mat3<real32> Mat3f;

template <typename T>
Mat3<T> QuatToMat3(const Quat<T>& q)
{
	Mat3<T> result(1);

	T qxx = (q.x * q.x);
	T qyy = (q.y * q.y);
	T qzz = (q.z * q.z);
	T qxz = (q.x * q.z);
	T qxy = (q.x * q.y);
	T qyz = (q.y * q.z);
	T qwx = (q.w * q.x);
	T qwy = (q.w * q.y);
	T qwz = (q.w * q.z);

	result.row0.x = 1 - (2) * (qyy + qzz);
	result.row0.y = (2) * (qxy + qwz);
	result.row0.z = (2) * (qxz - qwy);

	result.row1.x = (2) * (qxy - qwz);
	result.row1.y = (1) - (2) * (qxx + qzz);
	result.row1.z = (2) * (qyz + qwx);

	result.row2.x = (2) * (qxz + qwy);
	result.row2.y = (2) * (qyz - qwx);
	result.row2.z = (1) - (2) * (qxx + qyy);

	return result;
}

template <typename T>
T GetMatrixElement(const Mat3<T>& a, const int32& row, const int32& col)
{
	return a.ptr[3 * row + col];
}

template <typename T>
Vec3<T> GetColumn(const Mat3<T>& a, const int32& col)
{
	Vec3<T> column(0, 0, 0);

	column.x = a.ptr[3 * 0 + col];
	column.y = a.ptr[3 * 1 + col];
	column.z = a.ptr[3 * 2 + col];

	return column;
}

template <typename T>
Vec3<T> ScaleOfMatrix(const Mat3<T>& a)
{
	T x = Mag(a.row0);
	T y = Mag(a.row1);
	T z = Mag(a.row2);

	Vec3<T> result = Vec3<T>(x, y, z);

	return result;
}

template <typename T>
Mat3<T> RemoveScaleFromRotationMatrix(const Mat3<T>& a)
{
	Vec3<T> scale = ScaleOfMatrix(a);
	Mat3<T> result;

	for (int32 i = 0; i < 3; i++)
	{
		result[i] = a[i] / scale[i];
	}

	return result;
}

//template <typename T>
//inline String ToString(const Mat3<T>& a)
//{
//	String result;
//	result.Add('(');
//
//	for (int32 i = 0; i < 3; i++)
//	{
//		result.Add(a.ptr[i]).Add(" ");
//	}
//
//	result.Add("| ");
//
//	for (int32 i = 3; i < 6; i++)
//	{
//		result.Add(a.ptr[i]).Add(" ");
//	}
//
//	result.Add("| ");
//
//	for (int32 i = 6; i < 9; i++)
//	{
//		result.Add(a.ptr[i]).Add(" ");
//	}
//
//	result.Add(')');
//
//	return result;
//}

template <typename T>
inline T Det(const Mat3<T>& a)
{
	T f = a.ptr[0] * (a.ptr[4] * a.ptr[8] - a.ptr[7] * a.ptr[5]);
	T b = a.ptr[1] * (a.ptr[3] * a.ptr[8] - a.ptr[6] * a.ptr[5]);
	T c = a.ptr[2] * (a.ptr[3] * a.ptr[7] - a.ptr[6] * a.ptr[4]);

	return f - b + c;
}

template <typename T>
inline Mat3<T> Transpose(const Mat3<T>& m)
{
	Vec3<T> c0 = GetColumn(m, 0);
	Vec3<T> c1 = GetColumn(m, 1);
	Vec3<T> c2 = GetColumn(m, 2);

	Mat3<T> tra;
	tra.row0 = c0;
	tra.row1 = c1;
	tra.row2 = c2;

	return tra;
}

template <typename T>
inline Mat3<T> Inverse(const Mat3<T>& m)
{
	Mat3<T> inv;
	T d = Det(m);

	if (!Equal(d, static_cast<T>(0)))
	{
		Mat3<T> c;
		// @NOTE: Row 0 
		c[0][0] = (m[1][1] * m[2][2]) - (m[1][2] * m[2][1]);
		c[0][1] = -((m[1][0] * m[2][2]) - (m[1][2] * m[2][0]));
		c[0][2] = (m[1][0] * m[2][1]) - (m[1][1] * m[2][0]);

		// @NOTE: Row 1 
		c[1][0] = -((m[0][1] * m[2][2]) - (m[0][2] * m[2][1]));
		c[1][1] = (m[0][0] * m[2][2]) - (m[0][2] * m[2][0]);
		c[1][2] = -((m[0][0] * m[2][1]) - (m[0][1] * m[2][0]));

		// @NOTE: Row 2
		c[2][0] = (m[0][1] * m[1][2]) - (m[0][2] * m[1][1]);
		c[2][1] = -((m[0][0] * m[1][2]) - (m[0][2] * m[1][0]));
		c[2][2] = (m[0][0] * m[1][1]) - (m[0][1] * m[1][0]);

		inv = (static_cast<T>(1.0) / d) * Transpose(c);
	}

	return inv;
}

template <typename T>
inline Mat3<T> ScaleDirection(const Mat3<T>& a, const T& k, Vec3<T> direction)
{
	direction = Normalize(direction);

	Vec3<T> i_prime(0, 0, 0);
	i_prime.x = 1 + (k - 1) * direction.x * direction.x;
	i_prime.y = (k - 1) * direction.x * direction.y;
	i_prime.z = (k - 1) * direction.x * direction.z;

	Vec3<T> j_prime(0, 0, 0);
	j_prime.x = (k - 1) * direction.x * direction.y;
	j_prime.y = 1 + (k - 1) * direction.y * direction.y;
	j_prime.z = (k - 1) * direction.y * direction.z;

	Vec3<T> k_prime(0, 0, 0);
	k_prime.x = (k - 1) * direction.x * direction.z;
	k_prime.y = (k - 1) * direction.y * direction.z;
	k_prime.z = 1 + (k - 1) * direction.z * direction.z;

	Mat3<T> result(i_prime, j_prime, k_prime);

	return result;
}

template <typename T>
Mat3<T> Rotate(const Mat3<T>& a, const T& d_angle, Vec3<T> axis)
{
	axis = Normalize(axis);

	T theata = DegToRad(d_angle);
	T cos_theata = Cos(theata);
	T sin_theata = Sin(theata);

	Vec3<T> iPrime(0, 0, 0);
	iPrime.x = Round(axis.x * axis.x * (1 - cos_theata) + cos_theata);
	iPrime.y = Round(axis.x * axis.y * (1 - cos_theata) + axis.z * sin_theata);
	iPrime.z = Round(axis.x * axis.z * (1 - cos_theata) - axis.y * sin_theata);

	Vec3<T> jPrime(0, 0, 0);
	jPrime.x = Round(axis.x * axis.y * (1 - cos_theata) - axis.z * sin_theata);
	jPrime.y = Round(axis.y * axis.y * (1 - cos_theata) + cos_theata);
	jPrime.z = Round(axis.y * axis.z * (1 - cos_theata) + axis.x * sin_theata);

	Vec3<T> kPrime(0, 0, 0);
	kPrime.x = Round(axis.x * axis.z * (1 - cos_theata) + axis.y * sin_theata);
	kPrime.y = Round(axis.y * axis.z * (1 - cos_theata) - axis.x * sin_theata);
	kPrime.z = Round(axis.z * axis.z * (1 - cos_theata) + cos_theata);

	Mat3<T> result(iPrime, jPrime, kPrime);

	return a * result;
}

template <typename T>
Quat<T> Mat3ToQuat(const Mat3<T>& a)
{
	//@ HELP: 3D Math Primer for Graphics and Game Development
	T m11 = a.row0.x;
	T m12 = a.row0.y;
	T m13 = a.row0.z;

	T m21 = a.row1.x;
	T m22 = a.row1.y;
	T m23 = a.row1.z;

	T m31 = a.row2.x;
	T m32 = a.row2.y;
	T m33 = a.row2.z;

	T x2 = m11 - m22 - m33;
	T y2 = m22 - m11 - m33;
	T z2 = m33 - m11 - m22;
	T w2 = m11 + m22 + m33;

	int32 index = 0;
	T big2 = w2;

	if (x2 > big2)
	{
		big2 = x2;
		index = 1;
	}
	if (y2 > big2)
	{
		big2 = y2;
		index = 2;
	}
	if (z2 > big2)
	{
		big2 = z2;
		index = 3;
	}

	T big = Sqrt(big2 + static_cast<T>(1.0)) * static_cast<T>(0.5);
	T mult = static_cast<T>(0.25) / big;

	Quat<T> result;

	switch (index)
	{
	case 0:
	{
		T x = (m23 - m32) * mult;
		T y = (m31 - m13) * mult;
		T z = (m12 - m21) * mult;
		T w = big;

		result = Quat<T>(x, y, z, w);

		break;
	}
	case 1:
	{
		T x = big;
		T y = (m12 + m21) * mult;
		T z = (m31 + m13) * mult;
		T w = (m23 - m32) * mult;

		result = Quat<T>(x, y, z, w);

		break;
	}
	case 2:
	{
		T x = (m12 + m21) * mult;
		T y = big;
		T z = (m23 + m32) * mult;
		T w = (m31 - m13) * mult;

		result = Quat<T>(x, y, z, w);

		break;
	}
	case 3:
	{
		T x = (m31 + m13) * mult;
		T y = (m23 + m32) * mult;
		T z = big;
		T w = (m12 - m21) * mult;

		result = Quat<T>(x, y, z, w);

		break;
	}
	default:
		Assert(0, "INVAILD CODE PATH, Mat3ToQuat");
		return Quat<T>(0, 0, 0, 1);
	}

	return result;
}

template <typename T>
inline constexpr Mat3<T> operator+(const Mat3<T>& a, const Mat3<T>& b)
{
	Mat3<T> result;

	for (int32 i = 0; i < 9; i++)
	{
		result.ptr[i] = a.ptr[i] + b.ptr[i];
	}

	return result;
}

template <typename T>
inline constexpr Mat3<T> operator/(const Mat3<T>& a, const T& b)
{
	Mat3<T> result;

	for (int32 i = 0; i < 9; i++)
	{
		result.ptr[i] = a.ptr[i] / b;
	}

	return result;
}

template <typename T>
inline constexpr Mat3<T> operator/(const T& a, const Mat3<T>& b)
{
	Mat3<T> result;

	for (int32 i = 0; i < 9; i++)
	{
		result.ptr[i] = b.ptr[i] / a;
	}

	return result;
}

template <typename T>
inline constexpr Vec3<T> operator*(const Vec3<T>& a, const Mat3<T>& b)
{
	Vec3<T> result(0, 0, 0);

	for (uint32 i = 0; i < 3; i++)
	{
		Vec3<T> col = GetColumn(b, i);
		result[i] = Dot(col, a);
	}

	return result;
}

template <typename T>
inline constexpr Mat3<T> operator*(const Mat3<T>& a, const Mat3<T>& b)
{
	Mat3<T> result(1);

	for (int32 i = 0; i < 3; i++)
	{
		for (int32 y = 0; y < 3; y++)
		{
			Vec3<T> col(0, 0, 0);

			for (int32 x = 0; x < 3; x++)
			{
				col[x] = GetMatrixElement(b, x, y);
			}

			result.ptr[3 * i + y] = Dot(col, a[i]);
		}
	}

	return result;
}

template <typename T>
inline constexpr Mat3<T> operator*(const T& a, const Mat3<T>& b)
{
	Mat3<T> result;

	for (int32 i = 0; i < 9; i++)
	{
		result.ptr[i] = b.ptr[i] * a;
	}

	return result;
}

//************************************
// Matrix 4x4
//************************************

template <typename T>
struct Mat4
{
	union
	{
		T ptr[16];

		struct
		{
			Vec4<T> row0;
			Vec4<T> row1;
			Vec4<T> row2;
			Vec4<T> row3;
		};
	};

	Mat4()
	{
		this->row0 = Vec4<T>(1, 0, 0, 0);
		this->row1 = Vec4<T>(0, 1, 0, 0);
		this->row2 = Vec4<T>(0, 0, 1, 0);
		this->row3 = Vec4<T>(0, 0, 0, 1);
	}

	Mat4(const T& a)
	{
		this->row0 = Vec4<T>(a, 0, 0, 0);
		this->row1 = Vec4<T>(0, a, 0, 0);
		this->row2 = Vec4<T>(0, 0, a, 0);
		this->row3 = Vec4<T>(0, 0, 0, a);
	}

	Mat4(const Vec4<T>& row0, const Vec4<T>& row1, const Vec4<T>& row2, const Vec4<T>& row3)
	{
		this->row0 = row0;
		this->row1 = row1;
		this->row2 = row2;
		this->row3 = row3;
	}

	Mat4(const Mat3<T>& a, const Vec3<T>& translation)
	{
		this->row0 = Vec4<T>(a.row0, 0);
		this->row1 = Vec4<T>(a.row1, 0);
		this->row2 = Vec4<T>(a.row2, 0);
		this->row3 = Vec4<T>(translation, 1);
	}

	Mat4(const Mat3<T>& a, const Vec4<T>& b)
	{
		this->row0 = Vec4<T>(a.row0, 0);
		this->row1 = Vec4<T>(a.row1, 0);
		this->row2 = Vec4<T>(a.row2, 0);
		this->row3 = b;
	}

	Vec4<T>& operator[](const int32& index)
	{
		Assert(index >= 0 && index < 4, "Mat4 [] operator, invalid index");
		return (&row0)[index];
	}

	Vec4<T> operator[](const int32& index) const
	{
		Assert(index >= 0 && index < 4, "Mat4 [] operator, invalid index");
		return (&row0)[index];
	}
};

typedef Mat4<real32> Mat4f;
typedef Mat4<real64> Mat4d;

template <typename T>
inline Mat3<T> Mat4ToMat3(const Mat4<T>& a)
{
	Mat3<T> r = {};
	r.row0 = Vec4ToVec3(a.row0);
	r.row1 = Vec4ToVec3(a.row1);
	r.row2 = Vec4ToVec3(a.row2);

	return r;
}

template <typename T>
Vec3<T> ScaleOfMatrix(const Mat4<T>& a)
{
	T x = Mag(Vec4ToVec3(a.row0));
	T y = Mag(Vec4ToVec3(a.row1));
	T z = Mag(Vec4ToVec3(a.row2));

	Vec3<T> result = Vec3<T>(x, y, z);

	return result;
}

template <typename T>
Mat4<T> QuatToMat4(const Quat<T>& q)
{
	Mat4<T> result(1);

	T qxx = (q.x * q.x);
	T qyy = (q.y * q.y);
	T qzz = (q.z * q.z);
	T qxz = (q.x * q.z);
	T qxy = (q.x * q.y);
	T qyz = (q.y * q.z);
	T qwx = (q.w * q.x);
	T qwy = (q.w * q.y);
	T qwz = (q.w * q.z);

	result.row0.x = 1 - (2) * (qyy + qzz);
	result.row0.y = (2) * (qxy + qwz);
	result.row0.z = (2) * (qxz - qwy);

	result.row1.x = (2) * (qxy - qwz);
	result.row1.y = (1) - (2) * (qxx + qzz);
	result.row1.z = (2) * (qyz + qwx);

	result.row2.x = (2) * (qxz + qwy);
	result.row2.y = (2) * (qyz - qwx);
	result.row2.z = (1) - (2) * (qxx + qyy);

	return result;
}

template <typename T>
inline constexpr bool32 Equal(const Mat4<T>& a, const Mat4<T>& b, const T& epsilon = FLOATING_POINT_ERROR_PRESCION)
{
	bool32 result = true;

	for (int32 i = 0; result && i < 16; i++)
	{
		result = Equal(a.ptr[i], b.ptr[i], FLOATING_POINT_ERROR_PRESCION);
	}

	return result;
}

template <typename T>
inline constexpr T GetMatrixElement(const Mat4<T>& a, const int32& row, const int32& col)
{
	T result = a.ptr[4 * row + col];

	return result;
}

template <typename T>
inline constexpr Vec4<T> GetColumn(const Mat4<T>& a, const uint32& col)
{
	Vec4<T> result(0, 0, 0, 0);

	result.x = a.ptr[4 * 0 + col];
	result.y = a.ptr[4 * 1 + col];
	result.z = a.ptr[4 * 2 + col];
	result.w = a.ptr[4 * 3 + col];

	return result;
}

template <typename T>
inline constexpr Mat4<T> Transpose(const Mat4<T>& a)
{
	Vec4<T> c0 = GetColumn(a, 0);
	Vec4<T> c1 = GetColumn(a, 1);
	Vec4<T> c2 = GetColumn(a, 2);
	Vec4<T> c3 = GetColumn(a, 3);

	Mat4<T> result(c0, c1, c2, c3);

	return result;
}

template <typename T>
inline constexpr T Det(const Mat4<T>& a)
{
	T f = a.ptr[0] * (a.ptr[5] * (a.ptr[10] * a.ptr[15] - a.ptr[11] * a.ptr[14]) +
		a.ptr[6] * (a.ptr[11] * a.ptr[13] - a.ptr[9] * a.ptr[15]) +
		a.ptr[7] * (a.ptr[9] * a.ptr[14] - a.ptr[10] * a.ptr[13]));

	T b = a.ptr[1] * (a.ptr[4] * (a.ptr[10] * a.ptr[15] - a.ptr[11] * a.ptr[14]) +
		a.ptr[6] * (a.ptr[11] * a.ptr[12] - a.ptr[8] * a.ptr[15]) +
		a.ptr[7] * (a.ptr[8] * a.ptr[14] - a.ptr[10] * a.ptr[12]));

	T c = a.ptr[2] * (a.ptr[4] * (a.ptr[9] * a.ptr[15] - a.ptr[11] * a.ptr[13]) +
		a.ptr[5] * (a.ptr[11] * a.ptr[12] - a.ptr[8] * a.ptr[15]) +
		a.ptr[7] * (a.ptr[8] * a.ptr[13] - a.ptr[9] * a.ptr[12]));

	T d = a.ptr[3] * (a.ptr[4] * (a.ptr[9] * a.ptr[14] - a.ptr[10] * a.ptr[13]) +
		a.ptr[5] * (a.ptr[10] * a.ptr[12] - a.ptr[8] * a.ptr[14]) +
		a.ptr[6] * (a.ptr[8] * a.ptr[13] - a.ptr[9] * a.ptr[12]));

	T result = f - b + c - d;

	return result;
}

template <typename T>
inline constexpr Mat4<T> Translate(const Mat4<T>& a, const Vec3<T>& translation)
{
	Mat4<T> result = a;

	result.row3 = Vec4<T>(translation, 1) * a;

	return result;
}

template <typename T>
PolarCoord<T> Canonical(T r, T theta, T z)
{
	theta = DegToRad(theta);

	if (r == 0)
	{
		theta = 0;
	}
	else
	{
		if (r < 0.f)
		{
			r = -(r);
			theta += static_cast<T>(PI);
		}
		if (fabs(theta) > PI)
		{
			theta += static_cast<T>(PI);
			theta -= static_cast<T>(floor(theta / 2 * PI) * 2 * PI);
			theta -= static_cast<T>(PI);
		}
	}

	return { r, theta, z };
}

// @TODO: PolarCoords
template <typename T>
Mat4<T> Translate(Mat4<T> a, T length, T d_angle, T z)
{
	PolarCoord<T> p_coord = Canonical(length, d_angle, z);
	a.row3 = Vec4<T>(p_coord.r * Cos(p_coord.theta), p_coord.r * Sin(p_coord.theta), p_coord.z, 1) * a;
	return a;
}

// @TODO: PolarCoords
template <typename T>
Mat4<T> Translate(Mat4<T> a, PolarCoord<T> p_coord)
{
	a.row3 = Vec4<T>(p_coord.r * Cos(p_coord.theta), p_coord.r * Sin(p_coord.theta), p_coord.z, 1) * a;
	return a;
}

template <typename T>
inline constexpr Mat4<T> Rotate(const Mat4<T>& a, const T& d_angle, Vec3<T> axis)
{
	axis = Normalize(axis);

	T theata = DegToRad(d_angle);
	T cos_theata = Cos(theata);
	T sin_theata = Sin(theata);

	Vec4<T> iPrime(0, 0, 0, 0);
	iPrime.x = Round(axis.x * axis.x * (1 - cos_theata) + cos_theata);
	iPrime.y = Round(axis.x * axis.y * (1 - cos_theata) + axis.z * sin_theata);
	iPrime.z = Round(axis.x * axis.z * (1 - cos_theata) - axis.y * sin_theata);
	iPrime.w = 0;

	Vec4<T> jPrime(0, 0, 0, 0);
	jPrime.x = Round(axis.x * axis.y * (1 - cos_theata) - axis.z * sin_theata);
	jPrime.y = Round(axis.y * axis.y * (1 - cos_theata) + cos_theata);
	jPrime.z = Round(axis.y * axis.z * (1 - cos_theata) + axis.x * sin_theata);
	jPrime.w = 0;

	Vec4<T> kPrime(0, 0, 0, 0);
	kPrime.x = Round(axis.x * axis.z * (1 - cos_theata) + axis.y * sin_theata);
	kPrime.y = Round(axis.y * axis.z * (1 - cos_theata) - axis.x * sin_theata);
	kPrime.z = Round(axis.z * axis.z * (1 - cos_theata) + cos_theata);
	kPrime.w = 0;

	Vec4<T> wPrime(0, 0, 0, 1);

	Mat4<T> result(iPrime, jPrime, kPrime, wPrime);

	result = a * result;

	return result;
}

template <typename T>
inline constexpr Mat4<T> ScaleDirection(const Mat4<T>& a, const T& k, Vec3<T> unit_direction)
{
	unit_direction = Normalize(unit_direction);

	Vec4<T> iPrime(0, 0, 0, 0);
	iPrime.x = 1 + (k - 1) * unit_direction.x * unit_direction.x;
	iPrime.y = (k - 1) * unit_direction.x * unit_direction.y;
	iPrime.z = (k - 1) * unit_direction.x * unit_direction.z;

	Vec4<T> jPrime(0, 0, 0, 0);
	jPrime.x = (k - 1) * unit_direction.x * unit_direction.y;
	jPrime.y = 1 + (k - 1) * unit_direction.y * unit_direction.y;
	jPrime.z = (k - 1) * unit_direction.y * unit_direction.z;

	Vec4<T> kPrime(0, 0, 0, 0);
	kPrime.x = (k - 1) * unit_direction.x * unit_direction.z;
	kPrime.y = (k - 1) * unit_direction.y * unit_direction.z;
	kPrime.z = 1 + (k - 1) * unit_direction.z * unit_direction.z;

	Vec4<T> wPrime(0, 0, 0, 1);

	Mat4<T> result(iPrime, jPrime, kPrime, wPrime);

	result = result * a;

	return result;
}

template <typename T>
inline constexpr Mat3<T> ScaleCardinal(const Mat3<T>& a, const Vec3<T>& direction)
{
	Mat3<T> result = a;

	result.row0 = a.row0 * direction.x;
	result.row1 = a.row1 * direction.y;
	result.row2 = a.row2 * direction.z;

	return result;
}

template <typename T>
inline constexpr Mat4<T> ScaleCardinal(const Mat4<T>& a, const Vec3<T>& direction)
{
	Mat4<T> result = a;

	result.row0 = a.row0 * direction.x;
	result.row1 = a.row1 * direction.y;
	result.row2 = a.row2 * direction.z;

	return result;
}

template <typename T>
inline constexpr Mat4<T> CalculateTransformMatrix(const Vec3<T>& position, const Vec3<T>& scale, const Quat<T>& rotation)
{
	// @HELP: Real time rendering book, they are column-major btw
	Mat4<T> result;
	Mat4<T> trans(1);
	Mat4<T> rot(1);
	Mat4<T> scl(1);

	trans = Translate(trans, position);
	rot = QuatToMat4(rotation);
	scl = ScaleCardinal(scl, scale);

	result = scl * rot * trans;

	return result;
}

template <typename T>
bool32 CheckOrthogonal(const Mat4<T>& a, const T tolerance = 0.01)
{
	Mat4<T> result = a * Transpose(a);

	for (int32 i = 0; i < 4; i++)
	{
		if (abs(1 - abs(result.ptr[i * 5])) > tolerance)
		{
			return false;
		}
	}

	return true;
}

template <typename T>
Mat3<T> Adjoint(const Mat4<T>& a, const int32& row, const int32& col)
{
	Mat3<T> result(1);
	int32 index = 0;

	for (int32 r = 0; r < 4; r++)
	{
		if (row == r)
		{
			continue;
		}
		for (int32 c = 0; c < 4; c++)
		{
			if (c == col || c == col + 4 || c == col + 8 || c == col + 12)
			{
				continue;
			}

			result.ptr[index++] = GetMatrixElement(a, r, c);
		}
	}

	return result;
}

template <typename T>
inline constexpr Mat3<T> Minor(const Mat4<T>& a, const int32& i, const int32& j) {
	Mat3<T> minor = {};

	int32 yy = 0;
	for (int32 y = 0; y < 4; y++) {
		if (y == j) {
			continue;
		}

		int32 xx = 0;
		for (int32 x = 0; x < 4; x++) {
			if (x == i) {
				continue;
			}

			minor[xx][yy] = a[x][y];
			xx++;
		}

		yy++;
	}

	return minor;
}

template<typename T>
inline constexpr T Cofactor(const Mat4<T>& a, const int32& i, const int32& j)
{
	// @NOTE: The equation is not used here, but could be useful
	// @EQ: adj(a) = transpose(cofactor)

	Mat3<T> minor = Minor(a, i, j);

	real32 C = real32(Pow(static_cast<T>(-1.0), (real32)(i + 1 + j + 1))) * Det(minor);

	return C;
}

template <typename T>
Mat4<T> Inverse(const Mat4<T>& a)
{
	if (CheckOrthogonal(a))
	{
		return Transpose(a);
	}

	Mat4<T> result(1);
	Mat3<T> ad(1);
	int32 index = 0;

	for (int32 row = 0; row < 4; row++)
	{
		for (int32 col = 0; col < 4; col++)
		{
			if ((row + col) % 2)
			{
				ad = Adjoint(a, row, col);
				T i = -Det(ad);
				result.ptr[index++] = i;
			}
			else
			{
				ad = Adjoint(a, row, col);
				T i = Det(ad);
				result.ptr[index++] = i;
			}
		}
	}

	T determinant = Det(a);

	result = Transpose(result) / determinant;

	return result;
}

template <typename T>
Quat<T> Mat4ToQuat(const Mat4<T>& a)
{
	//@ HELP: 3D Math Primer for Graphics and Game Development
	T m11 = a.row0.x;
	T m12 = a.row0.y;
	T m13 = a.row0.z;

	T m21 = a.row1.x;
	T m22 = a.row1.y;
	T m23 = a.row1.z;

	T m31 = a.row2.x;
	T m32 = a.row2.y;
	T m33 = a.row2.z;

	T x2 = m11 - m22 - m33;
	T y2 = m22 - m11 - m33;
	T z2 = m33 - m11 - m22;
	T w2 = m11 + m22 + m33;

	int32 index = 0;
	T big2 = w2;
	if (x2 > big2)
	{
		big2 = x2;
		index = 1;
	}
	if (y2 > big2)
	{
		big2 = y2;
		index = 2;
	}
	if (z2 > big2)
	{
		big2 = z2;
		index = 3;
	}

	T big = Sqrt(big2 + static_cast<T>(1.0)) * static_cast<T>(0.5);
	T mult = static_cast<T>(0.25) / big;

	Quat<T> result;

	switch (index)
	{
	case 0:
	{
		T x = (m23 - m32) * mult;
		T y = (m31 - m13) * mult;
		T z = (m12 - m21) * mult;
		T w = big;

		result = Quat<T>(x, y, z, w);

		break;
	}
	case 1:
	{
		T x = big;
		T y = (m12 + m21) * mult;
		T z = (m31 + m13) * mult;
		T w = (m23 - m32) * mult;

		result = Quat<T>(x, y, z, w);

		break;
	}
	case 2:
	{
		T x = (m12 + m21) * mult;
		T y = big;
		T z = (m23 + m32) * mult;
		T w = (m31 - m13) * mult;

		result = Quat<T>(x, y, z, w);

		break;
	}
	case 3:
	{
		T x = (m31 + m13) * mult;
		T y = (m23 + m32) * mult;
		T z = big;
		T w = (m12 - m21) * mult;

		result = Quat<T>(x, y, z, w);

		break;
	}
	default:
		Assert(0, "Mat4ToQuat, no solution");
		return Quat<T>(0, 0, 0, 1);
	}

	return result;
}

//template <typename T>
//inline String ToString(const Mat4<T>& a)
//{
//	String result;
//	result.Add('(');
//	result.Add(ToString(a.row0));
//	result.Add(ToString(a.row1));
//	result.Add(ToString(a.row2));
//	result.Add(ToString(a.row3));
//	result.Add(')');
//	return result;
//}


template <typename T>
inline constexpr bool32 operator==(const Mat4<T>& a, const Mat4<T>& b)
{
	bool32 result = true;

	for (int32 i = 0; result && i < 16; i++)
	{
		result = a.ptr[i] == b.ptr[i];
	}

	return result;
}

template <typename T>
inline constexpr Mat4<T> operator/(const Mat4<T>& a, const T& b)
{
	Mat4<T> result;

	result.row0 = a.row0 / b;
	result.row1 = a.row1 / b;
	result.row2 = a.row2 / b;
	result.row3 = a.row3 / b;

	return result;
}

template <typename T>
inline constexpr Mat4<T> operator+(const Mat4<T>& a, const Mat4<T>& b)
{
	Mat4<T> result(1);

	result.row0 = a.row0 + b.row0;
	result.row1 = a.row1 + b.row1;
	result.row2 = a.row2 + b.row2;
	result.row3 = a.row3 + b.row3;

	return result;
}

template <typename T>
inline constexpr Mat4<T> operator-(const Mat4<T>& a, const Mat4<T>& b)
{
	Mat4<T> result(1);

	result.row0 = a.row0 - b.row0;
	result.row1 = a.row1 - b.row1;
	result.row2 = a.row2 - b.row2;
	result.row3 = a.row3 - b.row3;

	return result;
}

template <typename T>
inline constexpr Mat4<T> operator*(const Mat4<T>& a, const Mat4<T>& b)
{
	Mat4<T> result(1);

	for (int32 i = 0; i < 4; i++)
	{

		for (int32 y = 0; y < 4; y++)
		{

			Vec4<T> col(0, 0, 0, 0);
			for (int32 x = 0; x < 4; x++)
			{
				col[x] = GetMatrixElement(b, x, y);
			}

			result.ptr[4 * i + y] = Dot(col, a[i]);
		}
	}

	return result;
}

template <typename T>
inline constexpr Vec4<T> operator*(const Vec4<T>& a, const Mat4<T>& b)
{
	Vec4<T> result(0, 0, 0, 0);

	for (uint32 i = 0; i < 4; i++)
	{
		Vec4<T> col = GetColumn(b, i);
		result[i] = Dot(col, a);
	}

	return result;
}

//************************************
// Other Functions
//************************************

template <typename T>
inline constexpr Vec4<T> GetNormalisedDeviceCoordinates(const T& window_width, const T& window_height, const T& mouse_x, const T& mouse_y)
{
	// @NOTE: This is actualy clip space when the vec4 with -1 and 1
	T x = (static_cast<T>(2.0) * (mouse_x / window_width)) - static_cast<T>(1.0);
	T y = -((static_cast<T>(2.0) * (mouse_y / window_height)) - static_cast<T>(1.0));
	T z = static_cast<T>(-1.0);
	T w = static_cast<T>(1.0);

	Vec4<T> result = Vec4<T>(x, y, z, w);

	return result;
}

template <typename T>
inline constexpr Vec4<T> ToViewCoords(const Mat4<T>& projection_matrix, const Vec4<T>& viewCoords)
{
	Mat4<T> invproj = Inverse(projection_matrix);

	Vec4<T> result = viewCoords * invproj;

	return result;
}

template <typename T>
inline constexpr Vec3<T> ToWorldCoords(const Mat4<T>& view_matrix, const Vec4<T>& viewCoords)
{
	Mat4<T> invView = Inverse(view_matrix);
	Vec4<T> worldSpace = viewCoords * invView;
	Vec3<T> result = Vec3<T>(worldSpace.x, worldSpace.y, worldSpace.z);

	return result;
}

template <typename T>
Mat4<T> PerspectiveLH(const T& fov, const T& aspect, const T& znear, const T& zfar)
{
	T hfov = fov / static_cast<T>(2.0);
	T s = Sin(hfov);
	T c = Cos(hfov);

	T h = c / s;
	T w = h / aspect;
	T r = zfar / (zfar - znear);

	Mat4<T> result;
	result[0][0] = w;
	result[1][1] = h;
	result[2][2] = r;
	result[2][3] = static_cast<T>(1.0f);
	result[3][2] = static_cast<T>(-1.0f) * r * znear;
	result[3][3] = static_cast<T>(0.0f);

	return result;
}

template <typename T>
inline constexpr Mat4<T> PerspectiveRH(const T& dfovy, const T& aspect, const T& fnear, const T& ffar)
{
	// @TODO: Refactor as above (LH version).
	Mat4<T> result(1);

	T fovy = DegToRad(dfovy);
	T half_tan_fovy = Tan(fovy / static_cast<T>(2));

	result.row0 = Vec4<T>((static_cast<T>(1) / (aspect * half_tan_fovy)), static_cast<T>(0), static_cast<T>(0), static_cast<T>(0));
	result.row1 = Vec4<T>(static_cast<T>(0), (static_cast<T>(1) / half_tan_fovy), static_cast<T>(0), static_cast<T>(0));

	T a = -(ffar + fnear) / (ffar - fnear);
	T b = (static_cast<T>(-2) * ffar * fnear) / (ffar - fnear);

	result.row2 = Vec4<T>(static_cast<T>(0), static_cast<T>(0), a, static_cast<T>(-1));
	result.row3 = Vec4<T>(static_cast<T>(0), static_cast<T>(0), b, static_cast<T>(0));

	return result;
}

template <typename T>
inline constexpr Mat4<T> OrthographicRH(const T& left, const T& right, const T& top, const T& bottom, const T& _near, const T& _far)
{
	Mat4<T> result(1);

	result.row0 = Vec4<T>(static_cast<T>(2) / (right - left), static_cast<T>(0), static_cast<T>(0), static_cast<T>(0));
	result.row1 = Vec4<T>(static_cast<T>(0), static_cast<T>(2) / (top - bottom), static_cast<T>(0), static_cast<T>(0));
	result.row2 = Vec4<T>(static_cast<T>(0), static_cast<T>(0), static_cast<T>(-2) / (_far - _near), static_cast<T>(0));
	result.row3 = Vec4<T>(-(right + left) / (right - left), -(top + bottom) / (top - bottom), -(_far + _near) / (_far - _near), static_cast<T>(1));

	return result;
}

template <typename T>
inline constexpr Mat4<T> OrthographicLH(const T& left, const T& right, const T& top, const T& bottom, const T& _near, const T& _far)
{
	Mat4<T> result(1);

	result.row0 = Vec4<T>(static_cast<T>(2) / (right - left), static_cast<T>(0), static_cast<T>(0), static_cast<T>(0));
	result.row1 = Vec4<T>(static_cast<T>(0), static_cast<T>(2) / (top - bottom), static_cast<T>(0), static_cast<T>(0));
	// @NOTE: The '1' in this row in actaully d3d, in openg it would be 2 due to clip space being -1 to 1. I've leave it though
	result.row2 = Vec4<T>(static_cast<T>(0), static_cast<T>(0), static_cast<T>(1) / (_far - _near), static_cast<T>(0));
	result.row3 = Vec4<T>(-(right + left) / (right - left),
		-(top + bottom) / (top - bottom),
		_near / (_near - _far),
		static_cast<T>(1));

	return result;
}

template <typename T>
inline constexpr Mat4<T> LookAtLH(const Vec3<T>& position, const Vec3<T>& target, const Vec3<T>& up)
{
	Vec3<T> camera_reverse_direction = Normalize((position - target)); // @NOTE: LH

	Vec3<T> basis_right = Normalize(Cross(camera_reverse_direction, up));
	Vec3<T> basis_up = Cross(basis_right, camera_reverse_direction);
	Vec3<T> basis_forward = camera_reverse_direction * static_cast<T>(-1.0);

	Mat4<T> result;
	result.row0 = Vec4<T>(basis_right, static_cast<T>(0.0));
	result.row1 = Vec4<T>(basis_up, static_cast<T>(0.0));
	result.row2 = Vec4<T>(basis_forward, static_cast<T>(0.0));
	result.row3 = Vec4<T>(position, static_cast<T>(1.0));

	return result;
}

template <typename T>
inline constexpr Mat4<T> LookAtRH(const Vec3<T>& position, const Vec3<T>& target, const Vec3<T>& up)
{
	Vec3<T> camera_reverse_direction = Normalize((target - position)); // @NOTE: RH

	Vec3<T> basis_right = Normalize(Cross(camera_reverse_direction, up));
	Vec3<T> basis_up = Cross(basis_right, camera_reverse_direction);
	Vec3<T> basis_forward = camera_reverse_direction * static_cast<T>(-1.0);

	Mat4<T> result;
	result.row0 = Vec4<T>(basis_right, static_cast<T>(0.0));
	result.row1 = Vec4<T>(basis_up, static_cast<T>(0.0));
	result.row2 = Vec4<T>(basis_forward, static_cast<T>(0.0));
	result.row3 = Vec4<T>(position, static_cast<T>(1.0));

	return result;
}

template<typename T>
inline constexpr Mat3<T> ChangeOfBasisRHS(const Mat3<T>& outter, const Mat3<T>& inner)
{
	return Transpose(outter) * inner * outter;
}

template<typename T>
inline constexpr Mat3<T> ChangeOfBasisLHS(const Mat3<T>& outter, const Mat3<T>& inner)
{
	return outter * inner * Transpose(outter);
}


//************************************
// Basis Vectors
//************************************

template <typename T>
struct Basis
{
	union
	{
		Mat3<T> mat;
		struct
		{
			Vec3<T> right;
			Vec3<T> upward;
			Vec3<T> forward;
		};
	};

	Basis()
	{
		mat = Mat3<T>(1);
	}

	Basis(const Mat3f& orientation)
	{
		this->mat = orientation;
	}

	Basis(const Vec3<T>& right, const Vec3<T>& upward, const Vec3<T>& forward)
	{
		this->right = right;
		this->upward = upward;
		this->forward = forward;
	}
};

typedef Basis<real32> Basisf;

template <typename T>
inline Basis<T> Mat3ToBasis(const Mat3<T>& rotation_matrix) // @NOTE: Assumes it's a rotation matrix, ie orthgonal
{
	Basis<T> result;

	result.mat = rotation_matrix;

	return result;
}

template <typename T>
inline Basis<T> QuatToBasis(const Quat<T>& quat)
{
	Basis<T> result;

	result.mat = QuatToMat3(quat);

	return result;
}

//************************************
// Transform
//************************************

class Transform
{
public:
	Vec3f position = Vec3f(0, 0, 0);
	Vec3f scale = Vec3f(1, 1, 1);
	Quatf orientation = Quatf(0, 0, 0, 1);

public:
	inline void GlobalRotateX(const real32& rads)
	{
		orientation = ::GlobalRotateX(rads, orientation);
	}

	inline void GlobalRotateY(const real32& rads)
	{
		orientation = ::GlobalRotateY(rads, orientation);
	}

	inline void GlobalRotateZ(const real32& rads)
	{
		orientation = ::GlobalRotateZ(rads, orientation);
	}

	inline void LocalRotateX(const real32& rads)
	{
		orientation = ::LocalRotateX(rads, orientation);
	}

	inline void LocalRotateY(const real32& rads)
	{
		orientation = ::LocalRotateY(rads, orientation);
	}

	inline void LocalRotateZ(const real32& rads)
	{
		orientation = ::LocalRotateZ(rads, orientation);
	}

	inline Basisf GetBasis() const
	{
		Basisf result;

		result = QuatToBasis(this->orientation);

		return result;
	}

	inline Mat4f CalculateTransformMatrix() const
	{
		Mat4f result = ::CalculateTransformMatrix(this->position, this->scale, this->orientation);

		return result;
	}

	inline void LookAtLH(const Vec3f& point, const Vec3f& up = Vec3f(0.0f, 1.0f, 0.0f))
	{
		Mat4f look = ::LookAtLH(this->position, point, up);
		orientation = Mat4ToQuat(look);
	}

	inline void LookAtRH(const Vec3f& point, const Vec3f& up = Vec3f(0.0f, 1.0f, 0.0f))
	{
		Mat4f look = ::LookAtRH(this->position, point, up);
		orientation = Mat4ToQuat(look);
	}

	inline static Transform CombineTransform(const Transform& child, const Transform& parent)
	{
		Mat4f cm = child.CalculateTransformMatrix();
		Mat4f pm = parent.CalculateTransformMatrix();

		Mat4f r = cm * pm;

		Transform result(r);

		return result;
	}

public:
	Transform()
	{
	}

	explicit Transform(const Vec3f& position)
		: position(position)
	{
	}

	Transform(const Vec3f& position, const Vec3f& euler_rotation)
		: position(position)
	{
		orientation = EulerToQuat(euler_rotation);
	}

	Transform(const Vec3f& position, const Vec3f& euler_rotation, const Vec3f& scale)
		: position(position), scale(scale)
	{
		orientation = EulerToQuat(euler_rotation);
	}

	Transform(const Vec3f& position, const Quatf& orientation)
		: position(position), orientation(orientation)
	{
	}

	Transform(const Vec3f& position, const Quatf& orientation, const Vec3f& scale)
		: position(position), orientation(orientation), scale(scale)
	{
	}

	Transform(const Vec3f& position, const Basisf& basis)
		: position(position)
	{
		orientation = Mat3ToQuat(basis.mat);
	}

	explicit Transform(const Mat4f& m)
	{
		this->position.x = m.row3.x;
		this->position.y = m.row3.y;
		this->position.z = m.row3.z;

		this->scale = ScaleOfMatrix(m);

		// @NOTE: Is the same as calling RemoveScaleFromRotationMatrix, but we have scale so this is faster
		Mat3f rotationMatrix = Mat4ToMat3(m);
		for (int32 i = 0; i < 3; i++)
		{
			rotationMatrix[i] = rotationMatrix[i] / scale[i];
		}

		this->orientation = Mat3ToQuat(rotationMatrix);
	}
};

inline Vec3f RandomPointOnUnitSphere()
{
	// @HELP: http://corysimon.github.io/articles/uniformdistn-on-sphere/
	real32 r1 = RandomReal32();
	real32 r2 = RandomReal32();

	real32 theta = 2.0f * PI * r1;
	real32 phi = ArcCos(1.0f - 2.0f * r2);

	real32 sin_phi = Sin(phi);
	real32 cos_phi = Cos(phi);
	real32 cos_theta = Cos(theta);
	real32 sin_theta = Sin(theta);

	real32 x = sin_phi * cos_theta;
	real32 y = sin_phi * sin_theta;
	real32 z = cos_phi;

	Vec3f result = Vec3f(x, y, z);

	return result;
}

inline Vec3f RandomPointOnUnitHemisphere(const Vec3f& normal = Vec3f(0, 1, 0))
{
	Vec3f s = RandomPointOnUnitSphere();

	real32 d = Dot(s, normal);
	Vec3f result = d > 0 ? s : (static_cast<real32>(-1.0) * s);

	return result;
}



#pragma warning( pop )