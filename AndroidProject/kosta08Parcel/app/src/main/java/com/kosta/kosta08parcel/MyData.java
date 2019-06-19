package com.kosta.kosta08parcel;

import android.os.Parcel;
import android.os.Parcelable;

public class MyData implements Parcelable
{
    public String keyword;
    public int age;

    protected MyData(Parcel in)
    {
        keyword = in.readString();
        age = in.readInt();
    }

    public static final Creator<MyData> CREATOR=new Creator<MyData>()
    {
        @Override
        public MyData createFromParcel(Parcel in)
        {
            return new MyData(in);
        }

        @Override
        public MyData[] newArray(int size)
        {
            return new MyData[size];
        }
    };

    @Override
    public int describeContents()
    {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel parcel,int i)
    {
        parcel.writeString(keyword);
        parcel.writeInt(age);
    }
}
