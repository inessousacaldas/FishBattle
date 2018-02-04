// author:fish
using System;

public class Tuple
{
	public static Tuple<T1,T2> Create<T1,T2>(){
		return new Tuple<T1, T2>();
	}
	public static Tuple<T1,T2,T3> Create<T1,T2,T3>(){
		return new Tuple<T1, T2,T3>();
	}

	public static Tuple<T1,T2> Create<T1,T2>(T1 i1,T2 i2){
		return new Tuple<T1, T2>(i1,i2);
	}
	public static Tuple<T1,T2,T3> Create<T1,T2,T3>(T1 i1,T2 i2,T3 i3){
		return new Tuple<T1, T2,T3>(i1,i2,i3);
	}
	public static Tuple<T1,T2,T3,T4> Create<T1,T2,T3,T4>(T1 i1,T2 i2,T3 i3,T4 i4){
		return new Tuple<T1, T2,T3,T4>(i1,i2,i3,i4);
	}
}

public class Tuple<T1,T2> : IEquatable<Tuple<T1,T2>>{
	internal Tuple(){}
	internal Tuple (T1 i1,T2 i2){p1=i1;p2=i2;}
	public readonly T1 p1;
	public readonly T2 p2;

	public static bool operator ==(Tuple<T1,T2> a,Tuple<T1,T2> b){
		if ((object)a == null && (object)b == null) return true;
		if ((object)a != null && (object)b != null){
			return object.Equals(a.p1,b.p1) && object.Equals(a.p2,b.p2);
		}
		return false;
	}

	public static bool operator !=(Tuple<T1,T2> a,Tuple<T1,T2> b){
		return !(a == b);
	}

	//override object.Equals
	public override bool Equals (object obj)
	{
		Tuple<T1,T2> other = obj as Tuple<T1,T2>;
		return Equals(other);
	}

	//IEquatable<Wrapper<T>>
	public bool Equals (Tuple<T1,T2> other){
		return (object)other!=null && this == other;
	}

	//override object.GetHashCode
	public override int GetHashCode ()
	{
		if (p1 != null && p2 != null){
			return p1.GetHashCode() & p2.GetHashCode();
		}
		if (p1 != null) return p1.GetHashCode();
		if (p2 != null) return p2.GetHashCode();
		return 0;
	}

	public override string ToString ()
	{
		return string.Format ("Tuple<{0},{1}>:[{2}]<{3},{4}>",typeof(T1),typeof(T2), this.GetHashCode(), p1, p2);
	}
}

public class Tuple<T1,T2,T3>{
	internal Tuple(){}
	internal Tuple (T1 i1,T2 i2,T3 i3){p1=i1;p2=i2;p3=i3;}
	public readonly T1 p1;
	public readonly T2 p2;
	public readonly T3 p3;

	public static bool operator ==(Tuple<T1,T2,T3> a,Tuple<T1,T2,T3> b){
		if ((object)a == null && (object)b == null) return true;
		if ((object)a != null && (object)b != null){
			return object.Equals(a.p1,b.p1) && object.Equals(a.p2,b.p2) && object.Equals(a.p3,b.p3);
		}
		return false;
	}

	public static bool operator !=(Tuple<T1,T2,T3> a,Tuple<T1,T2,T3> b){
		return !(a == b);
	}

	//override object.Equals
	public override bool Equals (object obj)
	{
		Tuple<T1,T2,T3> other = obj as Tuple<T1,T2,T3>;
		return Equals(other);
	}

	//IEquatable<Wrapper<T>>
	public bool Equals (Tuple<T1,T2,T3> other){
		return (object)other!=null && this == other;
	}

	//override object.GetHashCode
	public override int GetHashCode ()
	{
		if (p1 == null && p2 == null && p3 == null){
			return 0;
		}
		int h1 = p1 != null ? p1.GetHashCode() : -1;
		int h2 = p2 != null ? p2.GetHashCode() : -1;
		int h3 = p3 != null ? p3.GetHashCode() : -1;
		return h1 & h2 & h3;
	}

	public override string ToString ()
	{
		return string.Format ("Tuple<{0},{1},{2}>:[{3}]<{4},{5},{6}>",typeof(T1),typeof(T2),typeof(T3),this.GetHashCode(), p1, p2,p3);
	}
}

public class Tuple<T1,T2,T3,T4>{
	internal Tuple(){}
	internal Tuple (T1 i1,T2 i2,T3 i3,T4 i4){p1=i1;p2=i2;p3=i3;p4=i4;}
	public readonly T1 p1;
	public readonly T2 p2;
	public readonly T3 p3;
	public readonly T4 p4;

	public static bool operator ==(Tuple<T1,T2,T3,T4> a,Tuple<T1,T2,T3,T4> b){
		if ((object)a == null && (object)b == null) return true;
		if ((object)a != null && (object)b != null){
			return object.Equals(a.p1,b.p1) && object.Equals(a.p2,b.p2) && object.Equals(a.p3,b.p3) && object.Equals(a.p4,b.p4);
		}
		return false;
	}

	public static bool operator !=(Tuple<T1,T2,T3,T4> a,Tuple<T1,T2,T3,T4> b){
		return !(a == b);
	}

	//override object.Equals
	public override bool Equals (object obj)
	{
		Tuple<T1,T2,T3,T4> other = obj as Tuple<T1,T2,T3,T4>;
		return Equals(other);
	}

	//IEquatable<Wrapper<T>>
	public bool Equals (Tuple<T1,T2,T3,T4> other){
		return (object)other!=null && this == other;
	}

	//override object.GetHashCode
	public override int GetHashCode ()
	{
		if (p1 == null && p2 == null && p3 == null){
			return 0;
		}
		int h1 = p1 != null ? p1.GetHashCode() : -1;
		int h2 = p2 != null ? p2.GetHashCode() : -1;
		int h3 = p3 != null ? p3.GetHashCode() : -1;
		int h4 = p4 != null ? p4.GetHashCode() : -1;
		return h1 & h2 & h3 & h4;
	}

	public override string ToString ()
	{
		return string.Format ("Tuple<{0},{1},{2},{3}>:[{4}]<{5},{6},{7},{8}>",typeof(T1),typeof(T2),typeof(T3),typeof(T4),this.GetHashCode(), p1, p2,p3,p4);
	}
}
