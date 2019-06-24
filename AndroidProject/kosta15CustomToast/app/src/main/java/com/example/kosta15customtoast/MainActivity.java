package com.example.kosta15customtoast;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.material.snackbar.Snackbar;

public class MainActivity extends AppCompatActivity
{
    EditText editX, editY;
    Button btnToast, btnChange, btnSnackbar;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        editX =(EditText)findViewById(R.id.editX);
        editY =(EditText)findViewById(R.id.editY);
        btnToast = (Button)findViewById(R.id.btnToast);
        btnChange = (Button)findViewById(R.id.btnChange);
        btnSnackbar = (Button)findViewById(R.id.btnSnackbar);
    }

    public void onBtnToast(View v)
    {
        Toast toastView = Toast.makeText(this, "위치가 바뀐 토스트 메시지 입니다.", Toast.LENGTH_SHORT);
        int xOffSet = Integer.parseInt(editX.getText().toString());
        int yOffSet = Integer.parseInt(editY.getText().toString());

        toastView.setGravity(Gravity.TOP|Gravity.TOP, xOffSet, yOffSet);
        toastView.show();
    }

    public void onBtnChange(View v)
    {
        LayoutInflater layoutInflater = getLayoutInflater();
        View layout = layoutInflater.inflate(R.layout.toastborder_layout, (ViewGroup)findViewById(R.id.toast_layout_root));
        TextView textView = layout.findViewById(R.id.textView);
        Toast toast = new Toast(this);
        textView.setText("모양 바꾼 토스트");
        toast.setGravity(Gravity.CENTER, 0, -100);
        toast.setDuration(Toast.LENGTH_LONG);
        toast.setView(layout);
        toast.show();
    }

    public void onBtnSnackbar(View v)
    {
        Snackbar.make(v, "스낵바입니다", Snackbar.LENGTH_LONG);
    }
}
