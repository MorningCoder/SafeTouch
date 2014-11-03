<?php
//下载图片页面
//用get方式获取
if(!isset($_GET['username'])||!isset($_GET['picture_name']))
{
	$error_msg = array('result' => '-1','message' => '下载失败！信息不完全');
		echo json_encode($error_msg);
			exit();    
			}

			$username = $_GET['username'];
			$picture_name = $_GET['picture_name'];

			$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
			if(!$link) 
			{
			    $error_msg = array();
				    $error_msg['result']='-1';
					    $error_msg['message']="连接失败！原因： " . mysql_error();
						    echo json_encode($error_msg);
							    exit();
								}
								//设置字符编码
								mysql_query("set names utf8",$link);
								//选择数据库
								if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
								{
								    $error_msg = array();
									    $error_msg['result']='-1';
										    $error_msg['message']="选择数据库失败！原因：" . mysql_error($link);
											    echo json_encode($error_msg);
												    exit();
													}

													$sql = "select * from Photo where username='$username' and picture_name='$picture_name'";
													$res = mysql_query($sql,$link);

													if($row = mysql_fetch_assoc($res))
													{
														header("Content-Type:image/jpg");
														    header('Content-Disposition: attachment; filename="'.$row['picture_name'].'"');  
																echo $row['picture'];   
																}
																else
																{
																	//文件不存在则输出空流
																	    echo "";
																		}
																		?>
