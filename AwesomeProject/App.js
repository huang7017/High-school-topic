/**
* Sample React Native App
* https://github.com/facebook/react-native
* @flow
*/

import React, { Component } from 'react';
import {
  AppRegistry,
  StyleSheet,
  Text,
  Image,
  View,
  WebView
} from 'react-native';

import MCSjs from 'react-native-mcsjs';
const remote = 'https://s15.postimg.org/tw2qkvmcb/400px.png';
a = require('./img/5.png');
b = require('./img/home.png');
texta = "安全";
textb = "沒有火災";
const myApp = MCSjs.register({
  appId: '682322283744088',
  appSecret: 'Z7XuPl5wnE44vPYY1OPG4knTvzzUNyNw',
  deviceId: 'Ds07057l',
});

export default class mcstest extends Component {
  constructor(props, context) {
    super(props, context);
    this.state = {
      message: 'NOT DERVID',
      message2: ' ',
      message3: ' ',
      message4: ' ',
    };
  }
  componentDidMount() {
    let _this = this;
    myApp.on('gas', function(data) {
      _this.setState({ message: data.updateDatapoint.values.value });
    });
    myApp.on('FIRE', function(data) {
      _this.setState({ message3: data.updateDatapoint.values.value });
    });
    myApp.on('sun', function(data) {
      _this.setState({ message2: data.updateDatapoint.values.value });
    });
    myApp.on('uv', function(data) {
      _this.setState({ message4: data.updateDatapoint.values.value });
    });
  }
  render() {
    if(this.state.message === 'Gas leak'){
      a = require('./img/gas.png');
      texta = "不安全";
    }
    else{
      a = require('./img/5.png');
      texta = "安全";
    }
    if(this.state.message3 === 'ON Fire'){
      b = require('./img/fire.png');
      textb = "火災";
    }
    else{
      b = require('./img/home.png');
      textb = "沒有火災";
    }
    return (
      <View style = {styles.container}>
        <View style = {styles.viewForTextStyle}>
          <Text style={styles.font}>
          瓦斯狀態:{"\n"}
          { texta }{"\n"}
          數值:{"\n"}
          { this.state.message2 }
          </Text>
        </View>
      <View style = {styles.viewForTextStyle2}>
          <Text style={styles.font}>
          {"\n"}火警狀態:{"\n"}
          { textb }
          </Text>
      </View>
      <Image
      style={{width: 50, height: 50 ,marginTop:10,marginLeft:60}}
      source={a}
      />
      <Image
      style={{width: 50, height: 50 ,marginTop:-50,marginLeft:260}}
      source={b}
      />
      <View style = {styles.viewForTextStyle3}>
          <Text style={styles.font2}>
          紫外線狀態:{ this.state.message4 }
          </Text>
      </View>
      <WebView
         source={{uri: 'https://drive.google.com/file/d/1OeNq7aCGE0rPKRBsLHaWCuPTZOU7uWkP/preview'}}
         style={{marginTop: 10}}
       />
      </View>
    );
  }
}
/*class TestBackgroundImage extends Component {
    render() {
        return (
            <BackgroundImage>
            </BackgroundImage>
        )
    }
}*/
const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  font:{
    color:'#000000',
    fontSize:30,
    fontWeight:'bold',
    justifyContent: 'center',
    alignItems: 'center',
  },
  font2:{
    color:'#000000',
    fontSize:30,
    fontWeight:'bold',
    justifyContent: 'center',
    alignItems: 'center',
  },
  viewForTextStyle:{
  height:160,
  width:170,
  flexDirection:'row',
  justifyContent:'center',
  backgroundColor:'#7acad3',
  marginTop:20,
  marginLeft:5,
  borderRadius: 10
  },
  viewForTextStyle2:{
    height:160,
    width:170,
    flexDirection:'row',
    justifyContent:'center',
    backgroundColor:'#d38970',
    marginTop:-160,
    marginLeft:185,
    borderRadius: 10
  },
  viewForTextStyle3:{
  height:50,
  width:345,
  flexDirection:'row',
  justifyContent:'center',
  backgroundColor:'#7A0099',
  marginTop:20,
  marginLeft:10,
  borderRadius: 10
  },
  img: {
    flex: 1,
    width: null,
    height: null,
    resizeMode: 'cover'
  },
});

AppRegistry.registerComponent('mcstest', () => mcstest);
