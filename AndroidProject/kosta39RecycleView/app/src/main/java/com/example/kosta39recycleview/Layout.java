package com.example.kosta39recycleview;

import android.content.Context;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

public class Layout extends LinearLayout
{
    ImageView imageView;
    TextView txtName, txtPhone;

    public Layout(Context context)
    {
        super(context);
        init(context);
    }

    public Layout(Context context,AttributeSet attrs)
    {
        super(context,attrs);
        init(context);
    }

    private void init(Context context)
    {
        LayoutInflater inflater = (LayoutInflater)context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        inflater.inflate(R.layout.layout, this, true);
        imageView = (ImageView)findViewById(R.id.imageView);
        txtName = (TextView)findViewById(R.id.txtName);
        txtPhone = (TextView)findViewById(R.id.txtPhone);
    }

    public void setImage(int resID)
    {
        imageView.setImageResource(resID);
    }

    public void setName(String name)
    {
        txtName.setText(name);
    }

    public void setPhone(String phone)
    {
        txtPhone.setText(phone);
    }
}
