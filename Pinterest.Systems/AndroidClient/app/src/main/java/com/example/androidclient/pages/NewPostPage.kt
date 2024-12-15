package com.example.androidclient.pages

import android.graphics.Bitmap
import android.graphics.ImageDecoder
import android.net.Uri
import android.widget.Toast
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.example.androidclient.AppNavigation
import com.example.androidclient.LocalConfig
import com.example.androidclient.components.TopBar
import com.example.androidclient.requests.NewPost
import com.example.androidclient.requests.addPost
import com.example.androidclient.storage
import kotlinx.coroutines.launch

@Composable
fun NewPostPage(navController: NavController) {
    var title by remember { mutableStateOf("") }
    var imageUri by remember { mutableStateOf<Uri?>(null) }
    var bitmap by remember { mutableStateOf<Bitmap?>(null) }

    val context = LocalContext.current
    val coroutineScope = rememberCoroutineScope()
    val config = LocalConfig.current
    val launcher = rememberLauncherForActivityResult(
        contract = ActivityResultContracts.GetContent()
    ) { uri: Uri? ->
        uri?.let {
            imageUri = it
            bitmap = run {
                val source = ImageDecoder.createSource(context.contentResolver, it)
                ImageDecoder.decodeBitmap(source)
            }
        }
    }
    Scaffold(
        topBar = {
            TopBar(
                title = "Добавление изображения",
                onBackClick = {
                    navController.navigate(AppNavigation.ImageGrid.route)
                }
            )
        },
        content = { padding -> Column(
                modifier = Modifier
                    .fillMaxSize()
                    .background(Color(0xFFF5F5F5))
                    .padding(padding)
                    .padding(30.dp,  30.dp),
                verticalArrangement = Arrangement.Top,
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                OutlinedTextField(
                    value = title,
                    onValueChange = { title = it },
                    label = { Text("Название изображения", color = Color.Black) },
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 16.dp),
                    singleLine = true,
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                    textStyle = LocalTextStyle.current.copy(color = Color.Black)
                )
                Button(
                    colors = ButtonDefaults.buttonColors(Color.Black),
                    contentPadding = PaddingValues(12.dp),
                    border = BorderStroke(1.dp, Color.Gray),
                    shape = RoundedCornerShape(10.dp),
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(top = 16.dp),
                    onClick = { launcher.launch("image/png")
                }) {
                    Text("Выбрать изображение", color = Color.White)
                }
                Spacer(modifier = Modifier.height(16.dp))
                if (bitmap != null) {
                    Image(
                        bitmap = bitmap!!.asImageBitmap(),
                        contentDescription = "Selected Image",
                        modifier = Modifier.size(200.dp)
                    )
                } else {
                    Text("No image selected", fontSize = 16.sp, color = Color.Black)
                }
                Spacer(modifier = Modifier.weight(1f))
                Button(
                    colors = ButtonDefaults.buttonColors(Color.Black),
                    contentPadding = PaddingValues(14.dp),
                    enabled = (bitmap != null && title.isNotEmpty()),
                    border = BorderStroke(1.dp, Color.Gray),
                    shape = RoundedCornerShape(10.dp),
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(top = 16.dp),
                    onClick = {
                        context.contentResolver.openInputStream(imageUri!!).use { stream ->
                            val bytes = stream?.readBytes() ?: byteArrayOf()
                            coroutineScope.launch {
                                addPost(config!!, storage["accessKey"]!!, NewPost(title, bytes)).let { result ->
                                    if (result) navController.navigate(AppNavigation.ImageGrid.route)
                                    else Toast.makeText(context, "Не удалось загрузить файл", Toast.LENGTH_LONG).show()
                                }
                            }
                        }
                    }
                ) {
                    Text("Загрузить", color = Color.White)
                }
            }
        }
    )
}