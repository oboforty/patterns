const MAX_IMAGE_SIZE = 1000000
const API_ENDPOINT = 'https://xe2vn2zmu6.execute-api.eu-central-1.amazonaws.com/file';
const ACCESS_TOKEN = '2NEPdRGOfmft1nAa8Ib3hkRPSvM9rCtrbNyZMUjUw4';

new Vue({
    el: "#app",
    data: {
      image: '',
      uploadURL: ''
    },
    methods: {
      onFileChange (e) {
        let files = e.target.files || e.dataTransfer.files
        if (!files.length) return
        this.createImage(files[0])
      },
      createImage (file) {
        // var image = new Image()
        let reader = new FileReader()
        reader.onload = (e) => {
          console.log('length: ', e.target.result.includes('data:image/png'))
          if (!e.target.result.includes('data:image/png')) {
            return alert('Wrong file type - PNG only.')
          }
          if (e.target.result.length > MAX_IMAGE_SIZE) {
            return alert('Image is loo large.')
          }
          this.image = e.target.result
        }
        reader.readAsDataURL(file)
      },
      removeImage: function (e) {
        console.log('Remove clicked')
        this.image = ''
      },
      uploadImage: async function (e) {
        console.log('Upload clicked')
        // Get the presigned URL
        const response = await axios({
          method: 'POST',
          url: API_ENDPOINT,
          crossdomain: true,
          headers: {
            Authorization: 'Bearer ' + ACCESS_TOKEN
          }
        })
        console.log('Response: ', response)
        //console.log('Uploading: ', this.image)

        let binary = atob(this.image.split(',')[1])
        let array = []
        for (var i = 0; i < binary.length; i++) {
          array.push(binary.charCodeAt(i))
        }

        // Upload file:
        let {url, fields} = response.opts.s3presignedurl;
        //let d = atob(fields.policy);
        let blobData = new Blob([new Uint8Array(array)], {type: 'image/png'})


        const formData = new FormData();
        //formData.append("Content-Type", "png");
//        Object.entries(fields).forEach(([key, value]) => {
//          formData.append(key, value);
//        });
        formData.append("Bucket", 'yanny-uploads-raw');
        //formData.append("Content-Type", 'image/png');
        //formData.append("acl", 'public-read');
        formData.append("key", fields.key);
        //formData.append("AWSAccessKeyId", fields.policy);
        formData.append("x-amz-signature", fields['x-amz-signature']);
        formData.append("x-amz-algorithm", fields['x-amz-algorithm']);
        formData.append("x-amz-credential", fields['x-amz-credential']);
        formData.append("x-amz-date", fields['x-amz-date']);
        formData.append("x-amz-security-token", fields['x-amz-security-token']);
        formData.append("policy", fields.policy);
        formData.append("file", blobData, "image.png");

        const res = await fetch(url, {
          method: "POST",
          headers: {
            //'Bucket': 'yanny-uploads-raw',
            //'Content-Type': "image/png",
            //'x-amz-acl': 'public-read'
          },
          body: formData,
        });

        const location = res.headers.get("Location"); // get the final url of our uploaded image

        this.uploadURL = decodeURIComponent(location);
        console.log(decodeURIComponent(location));

//x-amz-algorithm: "AWS4-HMAC-SHA256"
//x-amz-credential: "ASIAZIMS4WKY2CCJSLS5/20220108/eu-central-1/s3/aws4_request"
//x-amz-date: "20220108T235957Z"
//x-amz-security-token: "IQoJb3JpZ2luX2VjENj//////////wEaDGV1LWNlbnRyYWwtMSJIMEYCIQDmsCF+xApFVCnabHI75SM++R5gKmM6drkqhdlz54CJewIhAIEmvCitUBtcxYLqyj4cv1AFjtLSeT7y6RMDhm4k91+kKpkCCOH//////////wEQABoMNjM2NTAwMDk1NjY1Igw+pGQ2XLWTgFHnY7Uq7QGDeO7TgzqFFnLmwg44JFIUQjw7I9lyz3nl+yUqmDMMDeIggH0vnDGtXAtiN+PXtiZv0J+0C5OHqV9swR66GH688kz/H4dHj9I/nsrsZkglmQhvrqgA3dTQKPJqwQAFRNhhHL2mm39HEABSyNqS/6WigiWOZLOqhIyP3FX616fj1tR3WoXc0YidN45wHBB2TsRq6BtvxFTrx4cSJFdXSoKkVj4gjPBT5S0BUVwriNh/2x5IAOk9P0HdrRcazevpWnoFsQHi3YVWOxBsR6KoEzJMgGDB0aIkRr2goGFpKaW7L4UUVntPrFA5WzFwxZcwn8rojgY6mQH2kik5zhnLE2v95ICIPO3QCRV9jMeE4TTet6f1BCM3431fZN28PivpsBCc+I4km9QDeus7oNKetJoy+OjtwS254aGqb9N/C1nT4cYyJa8qjH7zyfwWTseVooXDngsO4hi8xqJH/Veve80UNY1DMaeiVtmuspPnPSSN2uZBP5ocZ/V5r5CDdYCKBTJNFvDZ/F4fYqx5j14v1eU="
//x-amz-signature: "bb8f689b25ca3dace6f56a99b9c071046e4ee44eaab05b1e79e884f6da9048e2"

/*
        url = url + fields['key'] + '?' + Object.entries(fields).map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(v)}`).join('&');

        console.log('Uploading to: ', url)
        const result = await fetch(url, {
          method: 'PUT',
          mode: 'cors',
          //headers: fields,
          body: blobData
        })
        console.log('Result: ', result)
        // Final URL for the user doesn't need the query string params
        this.uploadURL = url.split('?')[0]
*/
      }
    }
})