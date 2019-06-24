package com.example.kosta17customdialog;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import android.content.DialogInterface;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    TextView tvName, tvEmail;
    Button btnInput;
    EditText editName, editEmail;

    TextView toastText;
    View dialogView, toastView;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        tvName = (TextView)findViewById(R.id.tvName);
        tvEmail= (TextView)findViewById(R.id.tvEmail);
        btnInput = (Button)findViewById(R.id.btn_Input);

        btnInput.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                dialogView = (View)View.inflate(MainActivity.this, R.layout.dialog_layout, null);
                AlertDialog.Builder dlg = new AlertDialog.Builder(MainActivity.this);
                dlg.setTitle("사용자 정보 입력");
                dlg.setIcon(R.drawable.ic_menu_allfriends);
                dlg.setView(dialogView);
                dlg.setPositiveButton("확인",new DialogInterface.OnClickListener()
                {
                    @Override
                    public void onClick(DialogInterface dialogInterface,int i)
                    {
                        editName = (EditText)dialogView.findViewById(R.id.editName);
                        editEmail = (EditText)dialogView.findViewById(R.id.editEmail);
                        tvName.setText(editName.getText().toString());
                        tvEmail.setText(editEmail.getText().toString());
                    }
                });
                dlg.setNegativeButton("취소",new DialogInterface.OnClickListener()
                {
                    @Override
                    public void onClick(DialogInterface dialogInterface,int i)
                    {
                        Toast toast = new Toast(MainActivity.this);
                        toastView = (View)View.inflate(MainActivity.this, R.layout.toast_layout, null);
                        toastText = (TextView)toastView.findViewById(R.id.txtToast);
                        toastText.setText("취소했습니다");
                        toast.setView(toastView);
                        toast.show();
                    }
                });
                dlg.show();
            }
        });
    }
}
