<?php
//用于电脑端的登录检测
//检测手机是否上线同时验证密码
if(!isset($_POST['username'])||!isset($_POST['password']))
{
  $error_msg = array();
    $error_msg['result'] = '-1';
	  $error_msg['message'] = "登录失败！信息不完整";
	    echo json_encode($error_msg);
		  exit();
		  }
		  //赋值
		  $username = $_POST['username'];
		  $password = $_POST['password'];
		  //打开数据库连接
		  $link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
		  if(!$link) 
		  {
		      $error_msg = array();	
			      $error_msg['result']='-1';
				      $error_msg['message']="连接失败！原因： " . mysql_error();
					      echo json_encode($error_msg);
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
										  }
										  //构造用户名查询语句
										  $sql = "select * from UserInfo where username='".$username."'";
										  //执行语句
										  $res = mysql_query($sql,$link);
										  //读取该行结果
										  if($row = mysql_fetch_assoc($res))
										  {
										    if($row['password'] == $password)
											  {
											    	if($row['is_login'] == '1')
													    {
														    	$error_msg = array();
																    	$error_msg['result'] = "0";
																		    	$error_msg['message'] = "登录成功！手机客户端在线";
																				    	echo json_encode($error_msg);
																						    }
																							   	else if($row['is_login'] == '0')
																								    {
																									    	$error_msg = array();
																											    	$error_msg['result'] = "1";
																													    	$error_msg['message'] = "登录成功！手机客户端尚未在线";
																															    	echo json_encode($error_msg);
																																	    }
																																		  }
																																		    else
																																			  {
																																			      $error_msg = array();
																																				      $error_msg['result'] = "2";
																																					      $error_msg['message'] = "密码错误！";
																																						      echo json_encode($error_msg);
																																							    }
																																								}
																																								else
																																								{
																																								    $error_msg = array();
																																									    $error_msg['result'] = "2";
																																										    $error_msg['message'] = "账号不存在！";
																																											    echo json_encode($error_msg);
																																												}
																																												?>
