package com.example.kosta32sqlite;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

public class DBHelper extends SQLiteOpenHelper
{
    public DBHelper(Context context)
    {
        super(context, "Test.db", null, 1);
    }

    @Override
    public void onCreate(SQLiteDatabase sqLiteDatabase)
    {
        String sql = "create table TestTable( " + "idx integer primary key autoincrement, " +
                "textData text not null, " + "intData integer not null, " +
                "floatData real not null, " + "dateData date not null)";
        sqLiteDatabase.execSQL(sql);
    }

    @Override
    public void onUpgrade(SQLiteDatabase sqLiteDatabase, int oldVersion, int newVersion)
    {
        switch(oldVersion)
        {
        case 1:
        case 2:
        case 3:
        }
    }
}
