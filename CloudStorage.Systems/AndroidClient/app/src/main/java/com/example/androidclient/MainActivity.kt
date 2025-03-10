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
import androidx.compose.runtime.CompositionLocalProvider
import androidx.compose.runtime.compositionLocalOf
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.androidclient.pages.ImageGridPage
import com.example.androidclient.pages.LoginPage
import com.example.androidclient.pages.NewPostPage
import com.example.androidclient.pages.RegistrationPage
import com.example.androidclient.ui.theme.AndroidClientTheme
import com.example.androidclient.utils.Config
import com.example.androidclient.utils.readConfigFromFile

var storage = mutableMapOf<String, String>()

val LocalConfig = compositionLocalOf<Config?> { null }
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        val configuration = readConfigFromFile(this)
        setContent {
            AndroidClientTheme {
                CompositionLocalProvider(LocalConfig provides configuration) {
                    AppNavigation.NavComponent()
                }
            }
        }
    }
}

open class AppNavigation(val route: String) {
    companion object {
        @Composable
        fun NavComponent() {
            val navController = rememberNavController()
            NavHost(navController = navController, startDestination = Login.route) {
                composable(ImageGrid.route) { ImageGridPage(navController) }
                composable(NewPost.route) { NewPostPage(navController) }
                composable(Login.route) { LoginPage(navController) }
                composable(Register.route) { RegistrationPage(navController) }
            }
        }
    }
    object Login : AppNavigation("login")
    object Register : AppNavigation("register")
    object ImageGrid : AppNavigation("imageGrid")
    object NewPost : AppNavigation("newPost")
}