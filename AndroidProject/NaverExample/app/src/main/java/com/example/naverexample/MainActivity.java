package com.example.naverexample;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.nhn.android.naverlogin.OAuthLogin;
import com.nhn.android.naverlogin.OAuthLoginHandler;
import com.nhn.android.naverlogin.ui.view.OAuthLoginButton;

public class MainActivity extends AppCompatActivity
{
    OAuthLoginButton oAuthLoginButton;
    OAuthLogin mOAuthLoginModule;
    EditText txtID, txtPass;
    TextView textView;
    StringBuilder sb;
    private static Context mContext;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        sb = new StringBuilder();
        txtID = (EditText)findViewById(R.id.editId);
        txtPass = (EditText)findViewById(R.id.editPass);
        textView = (TextView)findViewById(R.id.textView);
        oAuthLoginButton = (OAuthLoginButton)findViewById(R.id.buttonOAuthLoginImg);
        oAuthLoginButton.setOAuthLoginHandler(mOAuthLoginHandler);
        mOAuthLoginModule = OAuthLogin.getInstance();
        mContext = this;
        oAuthLoginButton.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                mOAuthLoginModule.init(
                        mContext
                        ,txtID.getText().toString()
                        ,txtPass.getText().toString()
                        ,"네이버 로그인"
                );

                mOAuthLoginModule.startOauthLoginActivity(MainActivity.this, mOAuthLoginHandler);
            }
        });
    }

    /**
     * OAuthLoginHandler를 startOAuthLoginActivity() 메서드 호출 시 파라미터로 전달하거나 OAuthLoginButton
     객체에 등록하면 인증이 종료되는 것을 확인할 수 있습니다.
     */
    private OAuthLoginHandler mOAuthLoginHandler = new OAuthLoginHandler()
    {
        @Override
        public void run(boolean success)
        {
            if (success)
            {
                String accessToken = mOAuthLoginModule.getAccessToken(mContext);
                String refreshToken = mOAuthLoginModule.getRefreshToken(mContext);
                long expiresAt = mOAuthLoginModule.getExpiresAt(mContext);
                String tokenType = mOAuthLoginModule.getTokenType(mContext);
                sb.append("mOauthAT : ").append(accessToken).append("\n");
                sb.append("mOauthRT : ").append(refreshToken).append("\n");
                sb.append("mOauthExpires : ").append(expiresAt).append("\n");
                sb.append("mOauthTokenType : ").append(tokenType).append("\n");
                sb.append("mOAuthState : ").append(mOAuthLoginModule.getState(mContext).toString()).append("\n");
            }

            else
            {
                String errorCode = mOAuthLoginModule.getLastErrorCode(mContext).getCode();
                String errorDesc = mOAuthLoginModule.getLastErrorDesc(mContext);
                Toast.makeText(mContext, "errorCode:" + errorCode + ", errorDesc:" + errorDesc, Toast.LENGTH_SHORT).show();
            }
        };
    };

}
