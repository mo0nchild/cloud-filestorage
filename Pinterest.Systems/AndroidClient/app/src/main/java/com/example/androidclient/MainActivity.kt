package com.example.androidclient

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.androidclient.pages.ImageGridPage
import com.example.androidclient.pages.LoginPage
import com.example.androidclient.pages.RegistrationPage
import com.example.androidclient.ui.theme.AndroidClientTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            AndroidClientTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { _ ->
                    AppNavigation()
                }
            }
        }
    }
}

@Composable
fun AppNavigation() {
    // Создаем контроллер навигации
    val navController = rememberNavController()

    // Создаем NavHost с определением маршрутов
    NavHost(navController = navController, startDestination = "login") {
        composable("imageGrid") { ImageGridPage(navController) }
        composable("login") { LoginPage(navController) }
        composable("register") { RegistrationPage(navController) }
    }
}