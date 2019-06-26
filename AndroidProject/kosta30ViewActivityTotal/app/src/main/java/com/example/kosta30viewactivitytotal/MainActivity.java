package com.example.kosta30viewactivitytotal;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.fragment.app.Fragment;

import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.navigation.NavigationView;

public class MainActivity extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener, FragmentCallback
{
    Toolbar toolbar;
    Fragment1 fragment1;
    Fragment2 fragment2;
    Fragment3 fragment3;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        toolbar = findViewById(R.id.toolBar);
        setSupportActionBar(toolbar);

        DrawerLayout drawerLayout = findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawerLayout, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close
        );
        drawerLayout.addDrawerListener(toggle);
        toggle.syncState();
        NavigationView navigationView = findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);
        fragment1 = new Fragment1();
        getSupportActionBar().setTitle("첫번째 화면");
        getSupportFragmentManager().beginTransaction().add(R.id.mainContainer, fragment1).commit();

        BottomNavigationView bottomNavigationView = findViewById(R.id.bottom_navigation);
        bottomNavigationView.setOnNavigationItemSelectedListener(new BottomNavigationView.OnNavigationItemSelectedListener()
        {
            @Override
            public boolean onNavigationItemSelected(@NonNull MenuItem menuItem)
            {
                switch(menuItem.getItemId())
                {
                case R.id.tab1:
                    onFragmentSelected(0, null);
                    return true;

                case R.id.tab2:
                    onFragmentSelected(1, null);
                    return true;

                case R.id.tab3:
                    onFragmentSelected(2, null);
                    return true;
                }
                return false;
            }
        });
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item)
    {
        if(item.getItemId() == R.id.action_settings)
            return true;

        return super.onOptionsItemSelected(item);
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem menuItem)
    {
        int id = menuItem.getItemId();
        if(id == R.id.nav_0)
        {
            onFragmentSelected(0, null);
        }
        else if(id == R.id.nav_1)
        {
            onFragmentSelected(1, null);
        }
        else if(id == R.id.nav_2)
        {
            onFragmentSelected(2, null);
        }
        else if(id == R.id.nav_user_setting)
        {

        }
        DrawerLayout drawerLayout = findViewById(R.id.drawer_layout);
        drawerLayout.closeDrawer(GravityCompat.START);

        return true;
    }

    @Override
    public void onFragmentSelected(int position,Bundle bundle)
    {
        Fragment curFragment = null;
        if(position == 0)
        {
            curFragment = new Fragment1();
            toolbar.setTitle("첫번째 화면");
        }
        else if(position == 1)
        {
            curFragment = new Fragment2();
            toolbar.setTitle("두번째 화면");
        }
        else if(position == 2)
        {
            curFragment = new Fragment3();
            toolbar.setTitle("세번째 화면");
        }
        getSupportFragmentManager().beginTransaction().replace(R.id.mainContainer, curFragment).commit();
    }

    @Override
    public void onPointerCaptureChanged(boolean hasCapture)
    {

    }
}
