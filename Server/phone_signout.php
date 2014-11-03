<?php
//处理登出页面
if(!isset($_POST['username']))
{
  $error_msg = array();
    $error_msg['result'] = '-1';
	  $error_msg['message'] = "退出错误！信息不完整";
	    echo json_encode($error_msg);
		  exit();
		  }
		  //赋值
		  $username = $_POST['username'];
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
										  //构造更新语句
										  $update = "update UserInfo set is_login = 0 where username = '".$username."'";
										  //执行语句
										  $res = mysql_query($update,$link);
										  //读取该行结果
										  if($res)
										  {
										      $error_msg = array();
											      $error_msg['result'] = "0";
												      $error_msg['message'] = "退出成功";
													      echo json_encode($error_msg);
														  }
														  else
														  {
														      $error_msg = array();
															      $error_msg['result'] = "1";
																      $error_msg['message'] = "账号不存在！";
																	      echo json_encode($error_msg);
																		  }
																		  ?>
