package com.example.androidclient.pages

import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.input.VisualTransformation
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavHostController
import com.example.androidclient.AppNavigation
import com.example.androidclient.LocalConfig
import com.example.androidclient.requests.RegistrationInfo
import com.example.androidclient.requests.getAccessToken
import com.example.androidclient.requests.registrateAccount
import com.example.androidclient.storage
import kotlinx.coroutines.launch

@Composable
fun RegistrationPage(navController: NavHostController) {
    var email by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var passwordVisible by remember { mutableStateOf(false) }
    val config = LocalConfig.current
    val context = LocalContext.current
    val coroutineScope = rememberCoroutineScope()
    Column(
        modifier = Modifier
            .fillMaxSize()
            .background(Color(0xFFF5F5F5))
            .padding(16.dp),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = "Регистрация",
            fontSize = 32.sp,
            color = Color.Black,
            modifier = Modifier.padding(bottom = 32.dp)
        )

        OutlinedTextField(
            value = email,
            onValueChange = { email = it },
            label = { Text("Email", color = Color.Black) },
            modifier = Modifier
                .fillMaxWidth()
                .padding(bottom = 16.dp),
            singleLine = true,
            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
            textStyle = LocalTextStyle.current.copy(color = Color.Black)
        )

        OutlinedTextField(
            value = password,
            onValueChange = { password = it },
            label = { Text("Пароль", color = Color.Black) },
            modifier = Modifier
                .fillMaxWidth()
                .padding(bottom = 16.dp),
            singleLine = true,
            visualTransformation = if (passwordVisible) VisualTransformation.None else PasswordVisualTransformation(),
            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Password),
            textStyle = LocalTextStyle.current.copy(color = Color.Black),
            trailingIcon = {
                IconButton(onClick = { passwordVisible = !passwordVisible }) {
                    Icon(
                        painter = painterResource(
                            if (passwordVisible) android.R.drawable.ic_menu_view else android.R.drawable.ic_secure
                        ),
                        contentDescription = null
                    )
                }
            }
        )

        Button(
            onClick = {
                coroutineScope.launch {
                    registrateAccount(config!!, RegistrationInfo(email, password)).let { result ->
                        if (result != null) {
                            storage["accessKey"] = result.accessToken
                            storage["email"] = email
                            navController.navigate(AppNavigation.ImageGrid.route)
                        } else {
                            Toast.makeText(context, "Не удалось войти в аккаунт", Toast.LENGTH_LONG).show()
                        }
                    }
                }
            },
            colors = ButtonDefaults.buttonColors(Color.Black),
            contentPadding = PaddingValues(12.dp),
            border = BorderStroke(1.dp, Color.Gray),
            modifier = Modifier
                .fillMaxWidth()
                .padding(top = 16.dp)
        ) {
            Text("Зарегистроваться", color = Color.White)
        }


        TextButton(
            onClick = {
                navController.navigate("login")

            },
            modifier = Modifier.padding(top = 16.dp)
        ) {
            Text("У вас есть аккаунт? Войти", color = Color.Black)
        }
    }
}