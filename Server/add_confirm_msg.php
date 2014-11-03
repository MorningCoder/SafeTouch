<?php
//添加验证信息到ConfirmMsg表
//返回添加结果

//检验POST数据完整性
if(!isset($_POST['username']))
{
	$error_msg = array();
	  	$error_msg['result'] = '-1';
		  	$error_msg['message'] = "注册失败！信息不完整！";
			  	echo json_encode($error_msg);
				  	exit();
					}
					//赋值
					$username = $_POST['username'];
					//产生一个随机数
					$randnum = rand(10000,99999);
					//打开数据库连接
					$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
					if(!$link) 
					{
					  $error_msg = array();
					    $error_msg['result'] = '-1';
						  $error_msg['message'] = "连接失败！原因：" . mysql_error();
						    echo json_encode($error_msg);
							  exit();
							  }
							  //设置编码
							  mysql_query("set names utf8",$link);
							  //选择数据库
							  if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
							  {
							    $error_msg = array();
								  $error_msg['result'] = '-1';
								    $error_msg['message'] = "选择数据库失败！原因：" . mysql_error($link);
									  echo json_encode($error_msg);
									    exit();
										}
										//将新信息添加至ConfirmMsg表
										$res = mysql_query("insert into ConfirmMsg values ('$username','$randnum',0)",$link);
										//判断添加结果
										if(!$res)
										{
											$error_msg = array();
											  	$error_msg['result'] = '-1';
												  	$error_msg['message'] = "数据库执行失败！原因：" . mysql_error($link);
													  	echo json_encode($error_msg);
														   	exit();
															}
															//无错误则返回一个提示信息
															$res_msg = array('result' => '0','message' => '验证码即将发送到手机，请输入短信验证码');
															echo json_encode($res_msg);
															?>
