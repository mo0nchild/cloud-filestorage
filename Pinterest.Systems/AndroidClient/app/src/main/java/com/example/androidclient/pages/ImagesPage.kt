@file:Suppress("KotlinConstantConditions")

package com.example.androidclient.pages

import android.widget.Toast
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material3.Button
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.FloatingActionButton
import androidx.compose.material3.Icon
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.example.androidclient.AppNavigation
import com.example.androidclient.LocalConfig
import com.example.androidclient.components.ImageCard
import com.example.androidclient.components.MinimalModal
import com.example.androidclient.components.TopBar
import com.example.androidclient.requests.PostInfo
import com.example.androidclient.requests.SERVER_ADDRESS
import com.example.androidclient.requests.getAllPosts
import com.example.androidclient.storage
import com.example.androidclient.utils.downloadImageToFileSystem
import kotlinx.coroutines.launch
import java.util.UUID

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ImageGridPage(navController: NavController) {
    var images by remember { mutableStateOf<List<PostInfo>?>(null) }
    var showInfoModal by remember { mutableStateOf(false) }
    var reloadList by remember { mutableStateOf(UUID.randomUUID()) }
    val context = LocalContext.current
    val config = LocalConfig.current
    val coroutineScope = rememberCoroutineScope()
    LaunchedEffect(reloadList) {
        coroutineScope.launch {
            getAllPosts(config!!, storage["accessKey"]!!).let { result ->
                images = result?.map {
                    PostInfo(it.title, it.filePath.replace("localhost", SERVER_ADDRESS), it.uuid)
                }
                if (result == null) Toast.makeText(context, "Ошибка загрузки", Toast.LENGTH_LONG).show()
            }
        }
    }
    Scaffold(
        topBar = {
            TopBar(
                title = "Ваши изображения",
                onBackClick = {
                    storage.clear()
                    navController.navigate(AppNavigation.Login.route)
                },
                onInfoClick = { showInfoModal = true }
            )
        },
        floatingActionButton = {
            FloatingActionButton(
                onClick = {
                    navController.navigate(AppNavigation.NewPost.route)
                },
                containerColor = Color.Black,
                contentColor = Color.White
            ) {
                Icon(
                    imageVector = Icons.Default.Add,
                    contentDescription = "Add"
                )
            }
        },
        content = { padding ->
            if (showInfoModal) {
                MinimalModal("Информация об аккаунте\n${storage["email"]}", onDismissRequest = { showInfoModal = false })
            }
            if (images == null) {
                Column(
                    modifier = Modifier
                        .fillMaxSize()
                        .background(Color(0xFFF5F5F5))
                        .padding(padding),
                    horizontalAlignment = Alignment.CenterHorizontally,
                    verticalArrangement = Arrangement.Center
                ) {
                    Text("Не удалось загрузить изображения", color = Color.Black)
                    Spacer(Modifier.height(4.dp))
                    TextButton(
                        onClick = {
                            reloadList = UUID.randomUUID()
                        },
                    ) {
                        Text("Повторить", color = Color.Black, fontSize = 18.sp)
                    }
                }
            } else if (images != null && images!!.isEmpty()) {
                Column(
                    modifier = Modifier
                        .fillMaxSize()
                        .background(Color(0xFFF5F5F5))
                        .padding(padding),
                    horizontalAlignment = Alignment.CenterHorizontally,
                    verticalArrangement = Arrangement.Center
                ) {
                    Text("У вас еще нет изображений", color = Color.Black)
                }
            } else {
                LazyVerticalGrid(
                    columns = GridCells.Fixed(2),
                    contentPadding = PaddingValues(16.dp),
                    modifier = Modifier
                        .fillMaxSize()
                        .background(Color(0xFFF5F5F5))
                        .padding(padding)
                ) {
                    items(images!!) { item ->
                        Box(contentAlignment = Alignment.Center,
                            modifier = Modifier
                                .padding(10.dp)
                                .background(Color.Transparent)
                        ) {
                            ImageCard(title = item.title, imageUrl = item.filePath) {
                                downloadImageToFileSystem(context, item.filePath, "${item.title}.png")
                            }
                        }
                    }
                }
            }

        }
    )
}
