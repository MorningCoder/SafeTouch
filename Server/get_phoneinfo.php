<?php
//PC端获取特定手机号手机基本数据
if(!isset($_POST['username']))
{
	$error_msg = array('result' => '-1', 'message' => '信息不完全！');	
		echo json_encode($error_msg);
			exit();
			}

			$username = $_POST['username'];

			//打开数据库连接
			$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
			if(!$link) 
			{
			    $error_msg = array();
				    $error_msg['result'] = '-1';
					    $error_msg['message'] = "连接失败！原因： " . mysql_error();
						    echo json_encode($error_msg);
							    exit();
								}
								//设置字符编码
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

													$sql = "select * from PhoneInfo where username = '$username'";
													$res = mysql_query($sql,$link);
													//判断是否结果为空
													if(mysql_num_rows($res) == 0)
													{
													    $error_msg = array('result' => '*','message' => '不存在该用户信息');
														    echo json_encode($error_msg);
															    exit();
																}
																if($res)
																{
																	$row = mysql_fetch_assoc($res);
																		$arr = array('latitude' => $row['latitude'],'longtitude' => $row['longtitude'],'power' => $row['power'],'phone_name' => $row['phone_name']);
																			echo json_encode($arr);
																			} 
																			else
																			{
																				$error_msg = array('result' => '-1','message' => '获取失败：'.mysql_error());
																					echo json_encode($error_msg);
																					}
																					?>
